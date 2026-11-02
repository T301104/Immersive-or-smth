using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
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
			GameBoardObj = GameObject.Find("GameBoard");
			slidingGameManager = GameObject.Find("GameBoard").GetComponent<SlidingGameManager>();
			//Debug.Log(slidingGameManager.ToString());
			//slidingGameManager.players.Add(this);
			SlidingGameManager.OnStartupEvent += ReadyUp;

			myCam = Camera.main;
		}

	}

	public void ReadyUp(Transform gamePiece, float gapThickness, int row, int col, float width)
	{
		Debug.Log("readying up");
		float gap = gapThickness / 2;
		Mesh mesh = gamePiece.GetComponent<MeshFilter>().mesh;
		Vector2[] uv = new Vector2[4];

		uv[0] = new Vector2((width * col) + gap, 1 - ((width * (row + 1)) - gap));
		uv[1] = new Vector2((width * (col + 1)) - gap, 1 - ((width * (row + 1)) - gap));
		uv[2] = new Vector2((width * col) + gap, 1 - ((width * row) + gap));
		uv[3] = new Vector2((width * (col + 1)) - gap, 1 - ((width * row) + gap));

		mesh.uv = uv;
		//magic numbers cuz im brute forcing it
		//if ((row == slidingGameManager.gameSize.Value - 1) && (col == slidingGameManager.gameSize.Value - 1))
		//{
		//	slidingGameManager.emptyLocation = (slidingGameManager.gameSize.Value * slidingGameManager.gameSize.Value) - 1;
		//	gamePiece.gameObject.SetActive(false);
		//}
	}
}
