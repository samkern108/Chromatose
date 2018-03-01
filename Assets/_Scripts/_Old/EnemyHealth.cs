using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

	public GameObject deathExplosion;
	public bool invuln = true;
	private Animate animate;
	private Vector3 startSize, bigSize;

	private void Start() {
		animate = GetComponent <Animate>();
		startSize = transform.localScale;
		bigSize = startSize + (startSize * .2f);
	}

	public void OnTriggerEnter2D(Collider2D coll) {
		//Die ();
	}

	public void Die() {
		AudioManager.PlayEnemyDeath ();
		Instantiate (deathExplosion, transform.position, Quaternion.identity);
		GetComponent <Animate>().enabled = false;
		GameObject.Destroy (this.gameObject);
		if(GetComponent<GhostAI>())
			NotificationMaster.SendGhostDeathNotification (GetComponent<GhostAI>().stats);
	}

	public void StartMoving() {
		animate.AnimateToColor (Color.black, Color.white, .2f, Animate.RepeatMode.Once);
		//animate.AnimateToSize (startSize, bigSize, .2f, Animate.RepeatMode.Once);
		invuln = false;
	}

	public void StopMoving() {
		animate.AnimateToColor (Color.white, Color.black, .2f, Animate.RepeatMode.Once);
		//animate.AnimateToSize (bigSize, startSize, .2f, Animate.RepeatMode.Once);
		invuln = true;
	}
}
