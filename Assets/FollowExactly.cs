using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class FollowExactly : MonoBehaviour
    {

        public Transform objectToFollow;
        private Vector3 newPosition;

        void LateUpdate()
        {
            newPosition = objectToFollow.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }
}