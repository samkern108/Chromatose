using UnityEngine;
using System.Collections;

namespace Chromatose
{
    public class BlinkBall : MonoBehaviour, IBlinkObserver
    {
        private SpriteRenderer spriteRenderer;
		private Animate animate;
        private Vector3 startSize, bigSize;

        private void Start()
        {
			animate = GetComponent<Animate>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Blink()
        {
            spriteRenderer.color = Color.white;
			Invoke("EndBlinkEffect", Level.secondsPerBeat);
        }

		private void EndBlinkEffect() {
			spriteRenderer.color = Color.black;
		}
    }
}