using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using MovementEffects;

public class BackgroundColor : MonoBehaviour {

	[EventID]
	public string eventID;
	public float lastColorChangeFrame;
	private Color startColor, currentColor;
	public Color highlightColor;
	private string tag = "background";
	private SpriteRenderer spriteRenderer;

	private Color color1;
	private Color color2;

	private Koreography koreo;
	private KoreographyTrackBase track;
	private List<KoreographyEvent> kEvents;
	private int kIndex = 0;

	void Start()
	{
		Koreographer.Instance.RegisterForEventsWithTime(eventID, CameraColorEvent);
		spriteRenderer = GetComponent <SpriteRenderer>();
		startColor = spriteRenderer.color;
		currentColor = startColor;

		color1 = currentColor;
		color2 = highlightColor;

		koreo = Koreographer.Instance.GetKoreographyAtIndex(0);
		track = koreo.GetTrackByID(eventID);
		kEvents = track.GetAllEvents ();
	}

	void OnDestroy()
	{
		if (Koreographer.Instance != null)
		{
			Koreographer.Instance.UnregisterForAllEvents(this);
		}
	}
		
	void CameraColorEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
	{
		//Debug.Log (sampleTime + "  :  " + sampleDelta + "  :  " + deltaSlice.deltaOffset + "  :  " + deltaSlice.deltaLength);

		if (Time.frameCount != lastColorChangeFrame)
		{
			//int particleCount = (int)(particlesPerBeat * Koreographer.GetBeatTimeDelta());

			Timing.KillCoroutines (tag);
			int payload = ((IntPayload)evt.Payload).IntVal;
			Color newColor = (payload == 0) ? highlightColor : Color.cyan;

			float decayTime = (kEvents[kIndex + 1].StartSample - kEvents[kIndex].StartSample)/40000;
			Timing.RunCoroutine (C_AnimateToColor(currentColor, Color.cyan, .05f, decayTime), tag);
			kIndex++;

			/*Timing.RunCoroutine (C_AnimateToColor(color1, color2, .5f), tag);

			Color temp = color1;
			color1 = color2;
			color2 = temp;*/

			lastColorChangeFrame = Time.frameCount;
		}
	}

	/*private IEnumerator<float> C_AnimateToColor (Color start, Color finish, float inDuration) {
		float startTime = Time.time;
		float timer = 0;
		while(timer <= inDuration) {
			timer = Time.time - startTime;
			spriteRenderer.color = Color.Lerp (start, finish, timer/inDuration);
			currentColor = spriteRenderer.color;
			yield return 0;
		}
	}*/

	private IEnumerator<float> C_AnimateToColor (Color start, Color finish, float inDuration, float outDuration) {
		float startTime = Time.time;
		float timer = 0;
		while(timer <= inDuration) {
			timer = Time.time - startTime;
			spriteRenderer.color = Color.Lerp (start, finish, timer/inDuration);
			currentColor = spriteRenderer.color;
			yield return 0;
		}

		startTime = Time.time;
		timer = 0;
		while(timer <= outDuration) {
			timer = Time.time - startTime;
			spriteRenderer.color = Color.Lerp (finish, startColor, timer/outDuration);
			currentColor = spriteRenderer.color;
			yield return 0;
		}
	}
}
