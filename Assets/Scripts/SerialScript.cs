using JetBrains.Annotations;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using Unity.Netcode;
using UnityEngine;


public class SerialScript : NetworkBehaviour
{
	SerialPort stream;
	public NetworkVariable<int> valueOne;
	public NetworkVariable<int> valueTwo;
	public NetworkVariable<int> valueThree;

	public bool isControl = true;


	void Start()
	{
		stream = new SerialPort("COM4", 9600);
		valueOne = new NetworkVariable<int>();
		valueTwo = new NetworkVariable<int>();
		valueThree = new NetworkVariable<int>();
		stream.ReadTimeout = 50;
	}

	public override void OnNetworkSpawn ()
	{
		base.OnNetworkSpawn ();
		//valueOne.OnValueChanged += CheckErrorOne;
		//valueTwo.OnValueChanged += CheckErrorTwo;
		//valueThree.OnValueChanged += CheckErrorThree;
	}

		string tmp;
	void Update()
	{	
		if (!IsOwner || isControl)
		{
			return;
		}

		stream.Open();
		stream.WriteLine("P");
		stream.BaseStream.Flush();
		StartCoroutine(
			AsynchronousReadFromArduino(
			(string s) => tmp = s,
			() => Debug.LogError("Error!"),
			500f
			)
		);
		stream.Close();
		valueOne.Value = tmp[0] - '0';
		valueTwo.Value = tmp[1] - '0';
		valueThree.Value = tmp[2] - '0';
		//Debug.Log(value);

	}



	public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
	{
		DateTime initialTime = DateTime.Now;
		DateTime nowTime;
		TimeSpan diff = default(TimeSpan);
		string dataString = null;
		do
		{
			try
			{
				dataString = stream.ReadLine();
			}
			catch (TimeoutException)
			{
				dataString = null;
			}
			if (dataString != null)
			{
				callback(dataString);
				yield break;
			}
			else
				yield return null;
			nowTime = DateTime.Now;
			diff = nowTime - initialTime;
		} while (diff.Milliseconds < timeout);
		if (fail != null)
			fail();
		yield return null;
	}
}
