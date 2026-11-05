using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
	public Camera myCam;
	public AudioListener audioListener;
	[SerializeField] private GameObject UIPassword;

	[SerializeField] public SerialScript serialScript;

	private bool isPrison = false;
	private bool isControl = false;
	[SerializeField] private SlidingGameManager slidingGameManager;
	[SerializeField] private GameObject GameBoardObj;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		StartCoroutine(OwnerCheck());
	}
	private void Update()
	{
		if (!IsOwner)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.C) && !isPrison && !isControl)
		{
			isControl = true;
			UIPassword.SetActive(true);
			this.gameObject.tag = "Control";
		}
		if (Input.GetKeyDown(KeyCode.P) && !isPrison && !isControl)
		{
			this.gameObject.tag = "Prison";
			isPrison = true;
			serialScript.isControl = false;
			Debug.Log("trying to change pos");
			myCam.transform.position = new Vector3(77.55f,0,-10);
		}
	}
	IEnumerator OwnerCheck()
	{
		yield return new WaitForSeconds(1);
		if (!IsOwner)
		{
			Debug.Log("not owner of obj");
			
		}
		else
		{
			Debug.Log("There is an owner");
			UIPassword = GameObject.Find("GameManager").GetComponent<GameManager>().passwordUI;
			//GameBoardObj = GameObject.Find("GameBoard");
			//slidingGameManager = GameObject.Find("GameBoard").GetComponent<SlidingGameManager>();
			////Debug.Log(slidingGameManager.ToString());
			////slidingGameManager.players.Add(this);
			//SlidingGameManager.OnStartupEvent += ReadyUp;

			myCam.enabled = true;
			audioListener.enabled = true;
		}

	}

	//public void ReadyUp(Transform gamePiece, float gapThickness, int row, int col, float width)
	//{
	//	Debug.Log("readying up");
	//	float gap = gapThickness / 2;
	//	Mesh mesh = gamePiece.GetComponent<MeshFilter>().mesh;
	//	Vector2[] uv = new Vector2[4];

	//	uv[0] = new Vector2((width * col) + gap, 1 - ((width * (row + 1)) - gap));
	//	uv[1] = new Vector2((width * (col + 1)) - gap, 1 - ((width * (row + 1)) - gap));
	//	uv[2] = new Vector2((width * col) + gap, 1 - ((width * row) + gap));
	//	uv[3] = new Vector2((width * (col + 1)) - gap, 1 - ((width * row) + gap));

	//	mesh.uv = uv;
	//}
}
