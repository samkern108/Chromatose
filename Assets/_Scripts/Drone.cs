using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class Drone : MonoBehaviour {

	[EventID]
	public string eventID;
	private Koreography koreo;

	private float projectileSpeed = 7.0f;

	public GameObject projectile;
	private Animate animate;

	void Start () {
		Koreographer.Instance.RegisterForEventsWithTime(eventID, ShootEvent);
		animate = GetComponent <Animate>();
	}

	void ShootEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice) {
		//AudioManager.PlayEnemyShoot ();
		animate.AnimateToColor (Color.black, Color.white, .2f, Animate.RepeatMode.Once);
		Vector3 direction = (PlayerController.PlayerPosition - transform.position).normalized;
		GameObject missile = Instantiate (projectile, ProjectileManager.myTransform);
		missile.transform.position = transform.position;
		missile.GetComponent <Projectile>().Initialize(direction, projectileSpeed);
	}
}
