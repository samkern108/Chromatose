using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MovementEffects;

public class ColorWheel : MonoBehaviour {

	public Color[] colors = new Color[12];
	public GameObject smallWheelPrefab;
	public static ColorWheel self;

	// Red is 0
	// Blue is 1

	void Start () {
		self = this;
		CreateColorWheel ();
	}

	public void PieceClicked(ColorWheelPiece piece) {
		Timing.RunCoroutine (C_AnimateToColor(piece.color, 2.0f, piece.pieceIndex));
	}

	private IEnumerator<float> C_AnimateToColor (Color finish, float duration, int pieceIndex) {
		float startTime = Time.time;
		float timer = 0;
		while(timer <= duration) {
			timer = Time.time - startTime;
			Camera.main.backgroundColor = Color.Lerp (Color.black, finish, timer/duration);
			yield return 0;
		}
		SceneManager.LoadScene (2);
	}

	private void CreateColorWheel() {
		GameObject newPiece;
		SpriteRenderer spriteRenderer;
		float angle = 360/12;
		for (int i = 0; i < 12; i++) {
			newPiece = Instantiate (smallWheelPrefab, this.transform);
			newPiece.transform.position = new Vector3(0, 0, 0);

			//newPiece.transform.rotation = Quaternion.Euler (new Vector3(0, 0, i * angle));

			newPiece.transform.RotateAround(newPiece.transform.parent.position, new Vector3(0,0,1), i * angle);
			newPiece.transform.position = 1f * newPiece.transform.up;

			Vector2 targetVector = new Vector2 (0, 1);
			float angleRad = (i * angle) * Mathf.Deg2Rad;

			targetVector.x = targetVector.x * Mathf.Cos (angleRad) - targetVector.y * Mathf.Sin (angleRad);
			targetVector.y = targetVector.y * Mathf.Cos (angleRad) + targetVector.x * Mathf.Sin (angleRad);

			//newPiece.transform.position += (Vector3)targetVector;

			newPiece.GetComponent <ColorWheelPiece>().angle = (i * angle);

			spriteRenderer = newPiece.GetComponent <SpriteRenderer> ();
			spriteRenderer.sortingOrder = (i + 1) % 2;

			spriteRenderer.color = colors[i];
		}
	}
}
