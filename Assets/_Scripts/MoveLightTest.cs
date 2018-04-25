using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using System.Linq;

namespace Chromatose
{
    public class MoveLightTest : MonoBehaviour
    {

        private bool moving = false;
        private float moveTimeTotal = 0.0f;
        private float moveTimer = 0.0f;
        private int moveToPointIndex = 0;
        public int startPoint = 0;
        public bool snapToStartPoint = true;

        public Track track;

        void Start()
        {
            moveToPointIndex = startPoint;
            if (snapToStartPoint)
            {
                transform.position = track.points[moveToPointIndex].position;
                moveToPointIndex++;
            }

            moveFromPoint = transform.position;
            moveToPoint = track.points[moveToPointIndex].position;

            InvokeRepeating("TrackMoveEvent", 1f, 3f);
        }

        void TrackMoveEvent()
        {
            // This breaks if we only have one track while looping
            moving = true;
            moveTimer = 0.0f;
            moveTimeTotal = 2;

            moveFromPoint = transform.position;
            moveToPoint = track.points[moveToPointIndex].position;
            moveToPointIndex = (moveToPointIndex + 1) % track.points.Length;
        }

        private Vector3 moveFromPoint, moveToPoint;

        float diffX = .001f;
        float diffY = 0f;

        float percent = 0f;
        public void Update()
        {
            if (moving)
            {
                moveTimer += Time.deltaTime;
                if (moveTimer >= moveTimeTotal)
                {
                    moving = false;
                }
                else
                {
                    /*Vector3 newPosition = Vector3.Lerp(moveFromPoint, moveToPoint, moveTimer / moveTimeTotal);
                    Vector3 fuckYou = newPosition - moveFromPoint;
                    //Vector3 diff = transform.position + newPosition;
                    //Debug.Log(transform.position + "    " + (transform.position - newPosition));
					diffX += .001f;
                    transform.position += new Vector3(diffX, diffY, 0f); //Vector3.MoveTowards(transform.position, moveToPoint, .02f);//Vector3.Lerp(moveFromPoint, moveToPoint, moveTimer / moveTimeTotal);
                                                   //transform.position = Vector3.Lerp(moveFromPoint, moveToPoint, moveTimer / moveTimeTotal);*/

                                                   percent += .01f;

                   transform.position = (moveFromPoint + percent * (moveToPoint - moveFromPoint));
                }
            }
        }
    }
}