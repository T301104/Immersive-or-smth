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
	NetworkVariable<string> value;

	[SerializeField] GameObject errorOne;
	[SerializeField] GameObject errorTwo;
	[SerializeField] GameObject errorThree;

	void Start()
	{
		stream = new SerialPort("COM4", 9600);
		stream.ReadTimeout = 50;
	}


	void Update()
	{	
		stream.Open();
		stream.WriteLine("P");
		stream.BaseStream.Flush();
		StartCoroutine(
			AsynchronousReadFromArduino(
			(string s) => value.Value = s,
			() => Debug.LogError("Error!"),
			100f
			)
		);
		stream.Close();
		Debug.Log(value);

		int count = 0;

		if (value.Value[0] == '0')
		{
			errorOne.SetActive(true);
		}
		else
		{
			errorOne.SetActive(false);
			count++;
		}
		if (value.Value[1] == '0')
		{
			errorTwo.SetActive(true);
		}
		else
		{
			errorTwo.SetActive(false);
			count++;
		}
		if (value.Value[2] == '0')
		{
			errorThree.SetActive(true);
		}
		else
		{
			errorThree.SetActive(false);
			count++;
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
