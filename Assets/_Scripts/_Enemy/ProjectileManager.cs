using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class ProjectileManager : MonoBehaviour
    {

        public static Transform myTransform;

        // Can we recycle projectiles?

        void Start()
        {
            myTransform = transform;
        }

        public void Restart()
        {
            while (transform.childCount > 0)
            {
                Transform child = transform.GetChild(0);
                child.parent = null;
                Destroy(child.gameObject);
            }
        }
    }
}