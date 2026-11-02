using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class SlidingGameManager : NetworkBehaviour
{
    [SerializeField] private Transform gameLocation;
    [SerializeField] private NetworkObject gamePiece;

	public delegate void OnStartupDelegate(Transform gamePiece, float gapThickness, int row, int col, float width);
	public static event OnStartupDelegate OnStartupEvent;
	public List<Transform> pieces;
	public List<Transform> mirrorPieces;
	//public List<PlayerScript> players;
	private bool shuffling = false;
    public int emptyLocation;
    public NetworkVariable<int> gameSize;
	private bool gameDone = false;

    void Awake()
    {
		pieces = new List<Transform>();
		gameSize = new NetworkVariable<int>();
		gameSize.Value = 3;
	}
	public void StartPuzzle()
	{
		SettupGame(0.01f);
		if (!shuffling && CheckCompletion())
		{
			shuffling = true;
			StartCoroutine(WaitShuffle(0.5f));
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Debug.Log("sending ray");
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (hit)
			{
				// Go through the list, the index tells us the position.
				for (int i = 0; i < pieces.Count; i++)
				{
					if (pieces[i] == hit.transform)
					{
						// Check each direction to see if valid move.
						if (SwapIfValid(i, -gameSize.Value, gameSize.Value)) { break; }
						if (SwapIfValid(i, +gameSize.Value, gameSize.Value)) { break; }
						if (SwapIfValid(i, -1, 0)) { break; }
						if (SwapIfValid(i, +1, gameSize.Value - 1)) { break; }
					}
				}
			}
		}
	}


	// colCheck is used to stop horizontal moves wrapping.
	private bool SwapIfValid(int i, int offset, int colCheck)
	{
		if (((i % gameSize.Value) != colCheck) && ((i + offset) == emptyLocation))
		{
			// Swap them in game state.
			(pieces[i], pieces[i + offset]) = (pieces[i + offset], pieces[i]);
			// Swap their transforms.
			(pieces[i].localPosition, pieces[i + offset].localPosition) = ((pieces[i + offset].localPosition, pieces[i].localPosition));
			// Update empty location.
			emptyLocation = i;
			// Check for completion.

			if (!shuffling && CheckCompletion())
			{
				gameDone = true;
				//shuffling = true;
				//StartCoroutine(WaitShuffle(0.5f));
				CheckCompletion();
			}
			return true;
		}
		return false;
	}

	// pieces are named in order, check if order is correct
	private bool CheckCompletion()
	{
		for (int i = 0; i < pieces.Count; i++)
		{
			if (pieces[i].name != $"{i}")
			{
				return false;
			}
		}
		if (gameDone)
		{
			pieces[emptyLocation].gameObject.GetComponent<ComponentController>().SetEnabled(true);
		}
		return true;
	}

	private IEnumerator WaitShuffle(float duration)
	{
		yield return new WaitForSeconds(duration);
		Shuffle();
		shuffling = false;
	}

	// Brute force shuffling.
	private void Shuffle()
	{
		int count = 0;
		int last = 0;
		while (count < (gameSize.Value * gameSize.Value* gameSize.Value))
		{
			// Pick a random location.
			int rnd = Random.Range(0, gameSize.Value* gameSize.Value);
			Debug.Log(rnd);
			// Only thing we forbid is undoing the last move.
			if (rnd == last) { continue; }
			last = emptyLocation;
			// Try surrounding spaces looking for valid move.
			if (SwapIfValid(rnd, -gameSize.Value, gameSize.Value))
			{
				count++;
			}
			else if (SwapIfValid(rnd, +gameSize.Value, gameSize.Value))
			{
				count++;
			}
			else if (SwapIfValid(rnd, -1, 0))
			{
				count++;
			}
			else if (SwapIfValid(rnd, +1, gameSize.Value- 1))
			{
				count++;
			}
		}
	}
	private void SettupGame(float gapThickness)
    {
        float width = 1 / (float)gameSize.Value;
        for (int row = 0; row < gameSize.Value; row++)
        {
            for (int col = 0; col < gameSize.Value; col++)
            {
				NetworkObject pieceInstance = Instantiate(gamePiece, gameLocation);
				pieceInstance.GetComponent<NetworkObject>().Spawn();
				Transform piece = pieceInstance.transform;
				pieces.Add(piece);
				Transform mirrorImage = piece.gameObject.transform.GetChild(0);
				mirrorPieces.Add(mirrorImage);
				piece.localPosition = new Vector3(-1 + (2 * width * col) + width,
									+1 - (2 * width * row) - width,
									0);
				piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
				piece.name = $"{(row * gameSize.Value) + col}";

				OnStartupEvent?.Invoke(piece, gapThickness, row, col, width);
				if ((row == gameSize.Value - 1) && (col == gameSize.Value - 1) && IsHost)
				{
					piece.GetComponent<ComponentController>().SetEnabled(false);
					emptyLocation = (gameSize.Value * gameSize.Value) - 1;
					gamePiece.gameObject.SetActive(false);
				}
				//foreach (var x in players)
				//{
				//	x.ReadyUp(piece, gapThickness, row, col, width);
				//	x.ReadyUp(mirrorImage, gapThickness, row, col, width);
				//}
				//CreatePieceUV(piece, gapThickness, row, col, width);
				//CreatePieceUV(mirrorImage, gapThickness, row, col, width);

			}
		}
    }

 //   private void CreatePieceUV(Transform gamePiece, float gapThickness, int row, int col, float width)
 //   {
	//	if ((row == gameSize.Value- 1) && (col == gameSize.Value- 1))
	//	{
	//		emptyLocation = (gameSize.Value* gameSize) - 1;
	//		gamePiece.gameObject.SetActive(false);
	//	}
	//	else
	//	{
	//		float gap = gapThickness / 2;
	//		Mesh mesh = gamePiece.GetComponent<MeshFilter>().mesh;
	//		Vector2[] uv = new Vector2[4];

	//		uv[0] = new Vector2((width * col) + gap, 1 - ((width * (row + 1)) - gap));
	//		uv[1] = new Vector2((width * (col + 1)) - gap, 1 - ((width * (row + 1)) - gap));
	//		uv[2] = new Vector2((width * col) + gap, 1 - ((width * row) + gap));
	//		uv[3] = new Vector2((width * (col + 1)) - gap, 1 - ((width * row) + gap));

	//		mesh.uv = uv;
	//	}
	//}
}
