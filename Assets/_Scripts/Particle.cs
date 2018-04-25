using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour {

	public bool disableAfterDuration = false;

	void Start () {
		Invoke ("KillSelf", GetComponent<ParticleSystem>().main.duration - .1f);
	}
	
	void KillSelf () {
		if(disableAfterDuration) {
			gameObject.SetActive(false);
		}
		else
			Destroy (gameObject);
	}
}
