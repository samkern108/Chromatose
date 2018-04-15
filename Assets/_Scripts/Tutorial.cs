using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class Tutorial : MonoBehaviour
    {

        public GameObject keysBack, keysFront, spaceBack, spaceFront;

        void Start()
        {
            spaceBack.SetActive(false);
            keysBack.SetActive(true);
            Vector3 startPosition = keysFront.transform.position;
            Vector3 endPosition = keysFront.transform.position;
            endPosition.y -= 1f;
            keysFront.GetComponent<Animate>().AnimateToPosition(startPosition, endPosition, 2.0f, RepeatMode.PingPong);
        }
    }
}