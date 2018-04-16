using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class DumbBall : MonoBehaviour
    {
		private Color startColor = new Color(1f, 1f, 1f, 0f);
		private Animate animate;

        public void Spawn()
        {
			GetComponent<SpriteRenderer>().color = Color.black;
			animate = GetComponent<Animate>();
			//animate.AnimateToColor(Color.black, Color.white, Level.secondsPerBeat, RepeatMode.Once);
			animate.AnimateToSize(transform.localScale, (transform.localScale + transform.localScale * .6f), Level.secondsPerBeat, RepeatMode.OnceAndBack);
			Invoke("MakeWhite", Level.secondsPerBeat);
		}

		private void MakeWhite() {
			GetComponent<SpriteRenderer>().color = Color.white;
		}

		public void Update() {
			MoveToPlayer();
		}

		private void MoveToPlayer() {
			transform.position = Vector3.MoveTowards(transform.position, PlayerController.PlayerPosition, .005f);
		}
    }
}