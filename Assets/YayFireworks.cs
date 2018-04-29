using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class YayFireworks : MonoBehaviour
    {

        public GameObject yayFirework;

        void Start()
        {
			Invoke("MakeFirework", .2f);
        }

		private void MakeFirework() {
			GameObject firework = GameObject.Instantiate(yayFirework);
			firework.transform.position = BackgroundColor.GetRandomPointInRoom();
			Invoke("MakeFirework", Random.Range(.6f, 1.5f));
		}
    }
}