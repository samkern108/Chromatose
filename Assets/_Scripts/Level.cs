using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class Level : MonoBehaviour {

	[EventID]
	public string eventID;

	public static Koreography koreo;
	private static KoreographyTrackBase track;

	[HideInInspector]
	public static float BPM = 126;

	public static float secondsPerBeat, secondsPerMeasure;
	public static int levelID = (int)LevelColors.DarkBlue;

	void Awake () {
		Palette.InitColors ();

		koreo = Koreographer.Instance.GetKoreographyAtIndex(0);
		track = koreo.GetTrackByID(eventID);
		BPM = (float)koreo.GetBPM (0);
		/*float samplesPerBeat = (float)koreo.GetSamplesPerBeat (0);
		float songLength = koreo.SourceClip.length;
		float sampleRate = koreo.SampleRate;

		float totalBeats = (sampleRate * songLength) / samplesPerBeat;
		timePerBeat = totalBeats / songLength;*/
		secondsPerBeat = 60 / 126;
		secondsPerMeasure = secondsPerBeat * 4;
	}
}
