using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Chromatose
{
    public class IncreaseSpeed : MonoBehaviour, INotifyOnHitObserver
    {
		private float _startPitch, _endPitch;
        public float speedIncrement = .1f;
        public float beatMod = 2f;
        public void Start()
        {
			_startPitch = LevelMusic.audioSource.pitch;
			_endPitch = _startPitch;
			GetComponent<EnemyHealth>().notifyOnHitDelegates.Add(this);
        }

		public void NotifyOnHit(EnemyHealth health, bool dead) {
			_startPitch = _endPitch;
			_endPitch = _startPitch + speedIncrement;
            LevelMusic.ChangePitch(_endPitch, Level.secondsPerBeat * beatMod);
        }
    }
}