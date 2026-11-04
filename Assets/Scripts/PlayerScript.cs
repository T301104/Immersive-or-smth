using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
//using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
	public Camera myCam;
	public AudioListener audioListener;

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
		if (Input.GetKeyDown(KeyCode.V))
		{
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
