using UnityEngine;
using TMPro;
public class PaswordManager : MonoBehaviour
{
	private string inputText;
	public bool firstPress = true;
	public bool errorsSolved = false;

	[SerializeField] private GameObject wiresUI;
	[SerializeField] private GameObject inputFieldOBJ;
	[SerializeField] private GameObject UIBox;
	[SerializeField] private TMP_InputField inputField;
	[SerializeField] private string Password = "Fusion";
	[SerializeField] private string password = "fusion";

	public void OpenPasswordUI()
	{
		if (firstPress)
		{
			firstPress = false;
			wiresUI.SetActive(true);
		}
		else if (!inputFieldOBJ.activeSelf && errorsSolved)
		{
			inputFieldOBJ.SetActive(true);
			UIBox.SetActive(true);
		}
		else if (errorsSolved)
		{
			inputFieldOBJ.SetActive(false);
			UIBox.SetActive(false);
		}

	}
	public void CheckText(string input)
	{
		inputText = inputField.text;
		Debug.Log("checking password");
		Debug.Log("attempting: " + inputText);
		if (inputText == password || inputText == Password)
		{
			Debug.Log("password correct");
		}
	}
}
