using UnityEngine;

public class WiresManager : MonoBehaviour
{
	bool solvedOne = false;
	bool solvedTwo = false;
	bool solvedThree = false;

	[SerializeField] PaswordManager paswordManager;

	[SerializeField] SerialScript arduinoReader;



	private void Update()
	{
		if (arduinoReader == null)
		{
			arduinoReader = GameObject.FindWithTag("Prison").GetComponent<SerialScript>();
			if (arduinoReader != null)
			{
				Debug.Log("subscribing to events");
				arduinoReader.valueOne.OnValueChanged += CheckErrorOne;
				arduinoReader.valueTwo.OnValueChanged += CheckErrorTwo;
				arduinoReader.valueThree.OnValueChanged += CheckErrorThree;
			}
		}

		if (solvedOne && solvedTwo && solvedThree)
		{
			paswordManager.errorsSolved = true;
			this.gameObject.SetActive(false);
		}
	}
	private void CheckErrorOne(int old, int current)
	{
		Debug.Log("value is changed");

		if (current == 0)
		{
			errorOne.SetActive(true);
			solvedOne = false;
		}
		else
		{
			errorOne.SetActive(false);
			solvedOne = true;
		}
	}

	private void CheckErrorTwo(int old, int current)
	{
		if (current == 0)
		{
			errorTwo.SetActive(true);
			solvedTwo = false;
		}
		else
		{
			errorTwo.SetActive(false);
			solvedTwo = true;
		}
	}

	private void CheckErrorThree(int old, int current)
	{
		if (current == 0)
		{
			errorThree.SetActive(true);
			solvedThree = false;
		}
		else
		{
			errorThree.SetActive(false);
			solvedThree = true;
		}
	}
}
