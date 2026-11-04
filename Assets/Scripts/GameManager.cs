using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
	[SerializeField] GameObject ButtonOneUI;
	[SerializeField] GameObject ButtonTwoUI;
	[SerializeField] GameObject cameraStart;
	[SerializeField] NetworkObject slidingPuzzle;

	[SerializeField] public GameObject passwordUI;
	public event Action OnStartHost, OnStartClient, OnDiconnectClient;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	public void OffStartUI()
	{
		ButtonOneUI.SetActive(false);
		ButtonTwoUI.SetActive(false);
		cameraStart.SetActive(false);
	}
	public void DisconectClient()
	{
		NetworkManager.Shutdown();
	}

	public void StartClient()
	{
		NetworkManager.StartClient();
		OffStartUI();
		Debug.Log(NetworkManager.ConnectedClientsList);

	}

	public void StartHost()
	{
		Debug.Log("trying to host");
		NetworkManager.StartHost();
		OffStartUI();

		Debug.Log(NetworkManager.ConnectedClientsList);
		
	}
}
