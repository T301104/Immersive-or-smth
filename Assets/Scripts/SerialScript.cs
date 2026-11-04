using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using UnityEngine;


public class SerialScript : MonoBehaviour
{
	SerialPort stream;
	string value;

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
			(string s) => value = s,
			() => Debug.LogError("Error!"),
			100f
			)
		);
		stream.Close();
		Debug.Log(value[0]);
		Debug.Log(value[1]);
		Debug.Log(value[2]);
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
