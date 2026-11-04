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
	NetworkVariable<int> valueOne;
	NetworkVariable<int> valueTwo;
	NetworkVariable<int> valueThree;
	int count = 0;


	[SerializeField] GameObject errorOne;
	[SerializeField] GameObject errorTwo;
	[SerializeField] GameObject errorThree;

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
		valueOne.OnValueChanged += CheckErrorOne;
		valueTwo.OnValueChanged += CheckErrorTwo;
		valueThree.OnValueChanged += CheckErrorThree;
	}

		string tmp;
	void Update()
	{	
		stream.Open();
		stream.WriteLine("P");
		stream.BaseStream.Flush();
		StartCoroutine(
			AsynchronousReadFromArduino(
			(string s) => tmp = s,
			() => Debug.LogError("Error!"),
			100f
			)
		);
		stream.Close();
		valueOne.Value = tmp[0] - '0';
		valueTwo.Value = tmp[1] - '0';
		valueThree.Value = tmp[2] - '0';
		//Debug.Log(value);
	}

	private void CheckErrorOne(int old, int current)
	{
		Debug.Log("value is changed");

		if (current == 0)
		{
			errorOne.SetActive(true);
		}
		else
		{
			errorOne.SetActive(false);
			count++;
		}
	}

	private void CheckErrorTwo(int old, int current)
	{
		if (current == 0)
		{
			errorTwo.SetActive(true);
		}
		else
		{
			errorTwo.SetActive(false);
			count++;
		}

		if (count == 3)
		{
			this.gameObject.SetActive(false);
		}
	}

	private void CheckErrorThree(int old, int current)
	{
		if (current  == 0)
		{
			errorThree.SetActive(true);
		}
		else
		{
			errorThree.SetActive(false);
			count++;
		}
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
