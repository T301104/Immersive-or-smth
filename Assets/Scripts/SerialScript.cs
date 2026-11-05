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
	//public NetworkVariable<int> valueOne;
	//public NetworkVariable<int> valueTwo;
	//public NetworkVariable<int> valueThree;

	//public bool isControl = true;

	[SerializeField] GameObject errorOne;
	[SerializeField] GameObject errorTwo;
	[SerializeField] GameObject errorThree;


	void Start()
	{
		stream = new SerialPort("COM4", 9600);

		stream.ReadTimeout = 50;
	}

	//public override void OnNetworkSpawn ()
	//{
	//	base.OnNetworkSpawn ();

	//}

	//public void InitializeNetVars()
	//{
	//	if (IsOwner && !isControl)
	//	{
	//		Debug.Log("Initializing");
	//		valueOne = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
	//		valueOne.Initialize(this);
	//		valueTwo = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
	//		valueTwo.Initialize(this);
	//		valueThree = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
	//		valueThree.Initialize(this);
	//		valueOne.OnValueChanged += poopp;
	//	}
	//}


		string tmp;
	void Update()
	{	
		//if (!IsOwner || isControl)
		//{
		//	return;
		//}


		//Debug.Log("im gonna read");
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
		//if (timer >= 500)
		//{
		//	valueOne.Value = tmp[0] - '0';
		//	valueTwo.Value = tmp[1] - '0';
		//	valueThree.Value = tmp[2] - '0';
		//}
		//Debug.Log(value);
		int count = 0;
		if (tmp[0] == '1')
		{
			count++;
			errorOne.SetActive(false);
		}
		else
		{
			errorOne.SetActive(true);

		}
		if (tmp[1] == '1')
		{
			count++;
			errorTwo.SetActive(false);

		}
		else
		{
			errorTwo.SetActive(true);

		}
		if (tmp[2] == '1')
		{
			count++;
			errorThree.SetActive(false);

		}
		else
		{
			errorThree.SetActive(true);
		}
		if (count == 3)
		{
			this.gameObject.SetActive(false);
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
