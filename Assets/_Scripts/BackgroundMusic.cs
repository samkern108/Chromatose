using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour {

	private AudioSource backgroundMusic;
	public AudioSource metronome;

	[HideInInspector]
	public float startTime;
	public static float BPM = 126;
	public float BPM_HALF = BPM/2;
	public float BPM_DOUBLE = BPM * 2;
	public float BEATSPACING = 60/BPM;

	// this is in half steps
	//public int[] onOff = {2, 0, 1, 1, 0, 0, 0, 0, 2, 0, 1, 0, 1, 0, 0, 0 };
	public int[] onOff = {2, 0, 0, 0, 1, 0, 1, 0, 2, 0, 1, 0 };

	public int BEAT = 0, MEASURE_LENGTH;

	void Start () {
		// Uh this is not mine... it's by Slow Magic....... I just... like it a lot >___<
		//backgroundMusic = GetComponent<AudioSource>();
		//backgroundMusic.Play ();
		startTime = Time.deltaTime;

		MEASURE_LENGTH = onOff.Length;

		//InvokeRepeating ("SendTick", BEATSPACING / 2, BEATSPACING * 2);
	}

	private void SendTick() {
		NotificationMaster.SendTickNotification (onOff[BEAT]);
		metronome.Play ();
		BEAT = (BEAT + 1) % MEASURE_LENGTH;
	}

	void Update () {
		
	}
}
