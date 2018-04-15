using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Chromatose
{
    public class ScreenShake : MonoBehaviour
    {

        private float normalSize;
        private Vector3 normalPosition;

        public void Start()
        {
            normalSize = Camera.main.orthographicSize;
            normalPosition = Camera.main.transform.position;
        }

        // pulse should take next beat time into account I'm so tired
        public void Pulse(float val)
        {
            val = Mathf.Clamp(val * normalSize, 3 * normalSize / 4, normalSize);
            Timing.RunCoroutine(C_AnimateToSize(val, .2f, true));
        }

        private IEnumerator<float> C_AnimateToSize(float size, float duration, bool andBack)
        {
            Vector3 newPosition = normalPosition;
            float startTime = Time.time;
            float timer = 0;
            float sizeDiff = 0;
            while (timer <= duration)
            {
                timer = Time.time - startTime;
                Camera.main.orthographicSize = Mathf.Lerp(normalSize, size, timer / duration);
                sizeDiff = 10 * (normalSize - Camera.main.orthographicSize) / ((normalSize + Camera.main.orthographicSize) / 2);
                newPosition = normalPosition + (PlayerController.PlayerPosition - normalPosition).normalized * sizeDiff;
                newPosition.z = normalPosition.z;
                transform.position = newPosition;
                yield return 0;
            }
            startTime = Time.time;
            timer = 0;
            while (timer <= duration)
            {
                timer = Time.time - startTime;
                Camera.main.orthographicSize = Mathf.Lerp(size, normalSize, timer / duration);
                sizeDiff = 10 * (normalSize - Camera.main.orthographicSize) / ((normalSize + Camera.main.orthographicSize) / 2);
                newPosition = normalPosition + (PlayerController.PlayerPosition - normalPosition).normalized * sizeDiff;
                newPosition.z = normalPosition.z;
                transform.position = newPosition;
                yield return 0;
            }
        }
    }
}