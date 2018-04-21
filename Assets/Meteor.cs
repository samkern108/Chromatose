using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class Meteor : MonoBehaviour
    {

		private Animate _animate;
        private float speed;

        void Start()
        {
			_animate = GetComponent<Animate>();
            // the meteor should take 2 beats to cross the entire level.
			float mod = (Level.secondsPerBeat) * Mathf.Floor(Random.Range(2, 6));
            speed = BackgroundColor.bounds.size.y / mod;
    
			GetComponent<SpriteRenderer>().color = Color.black;
			//_animate.AnimateToSize(transform.localScale, (transform.localScale + transform.localScale * .6f), Level.secondsPerBeat, RepeatMode.OnceAndBack);
			Invoke("MakeWhite", Level.secondsPerBeat);
		}

		private void MakeWhite() {
			GetComponent<SpriteRenderer>().color = Color.white;
		}

        void Update()
        {
            transform.position += Vector3.down * speed  * Time.deltaTime;
        }
    }
}