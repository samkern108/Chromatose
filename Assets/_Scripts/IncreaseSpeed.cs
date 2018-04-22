﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Chromatose
{
    public class IncreaseSpeed : MonoBehaviour, INotifyOnHitObserver
    {
		private float _startPitch, _endPitch;
        public void Start()
        {
			Level.audioSource.pitch = .4f;
			_startPitch = Level.audioSource.pitch;
			_endPitch = _startPitch;
			GetComponent<EnemyHealth>().notifyOnHitDelegate = this;
        }

		public void NotifyOnHit(bool dead) {
			_startPitch = _endPitch;
			_endPitch = _startPitch + .1f;
			Timing.RunCoroutine(C_ChangePitch(Level.secondsPerBeat * 2f));
        }

        private IEnumerator<float> C_ChangePitch(float time)
        {
            float startTime = Time.time;
            float timer = 0;
            while (timer <= time)
            {
                timer = Time.time - startTime;
                Level.audioSource.pitch = Mathf.Lerp(_startPitch, _endPitch, timer / time);
                yield return 0;
            }
        }
    }
}