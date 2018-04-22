using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

namespace Chromatose
{
    public class LaserMovement : MonoBehaviour
    {
        [EventID]
        public string eventID;
        private Koreography koreo;

        private bool moving = false;
        private float moveTimeTotal = 0.0f;
        private float moveTimer = 0.0f;

        public Track track;
        private int moveToPointIndex = 0;
        private int samplesPerBeat;
        public bool snapToStartPoint = false;

        // Directional locks when moving toward the player
        public bool lockY = false;
        public bool lockX = false;

        void Start()
        {
            Koreographer.Instance.RegisterForEventsWithTime(eventID, TrackMoveEvent);
            moveToPointIndex = 0;
            if (snapToStartPoint)
            {
                transform.position = track.points[moveToPointIndex].position;
                moveToPointIndex++;
            }

            samplesPerBeat = (int)Level.koreo.GetSamplesPerBeat(0);

            moveFromPoint = transform.position;
            moveToPoint = track.points[moveToPointIndex];

            koreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        }

        void OnDestroy()
        {
            if (Koreographer.Instance != null)
                Koreographer.Instance.UnregisterForAllEvents(this);
        }

        private int currentSampleTime = 0;
        void TrackMoveEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            if (currentSampleTime != evt.StartSample)
            {
                currentSampleTime = evt.StartSample;
                moving = true;
                moveTimer = 0.0f;
                moveTimeTotal = ((evt.EndSample - evt.StartSample) / samplesPerBeat);

                int payload = evt.HasIntPayload() ? payload = ((IntPayload)evt.Payload).IntVal : 0;
				movementType = payload;
                if (payload == 0)
                {
                    moveFromPoint = transform.position;
                    //moveFromPoint = track.points[pointIndex].position;
                    moveToPoint = track.points[moveToPointIndex];
                    moveToPointIndex = (moveToPointIndex + 1) % track.points.Length;
                }
                else if (payload == 1)
                {
                    moveFromPoint = transform.position;
                    moveToPoint = PlayerController.PlayerTransform;
                }
            }
        }

        private Vector3 moveFromPoint;
        private Transform moveToPoint;

        private int movementType = 0;

        public void Update()
        {
            if (moving)
            {
                moveTimer += koreo.GetBeatTimeDelta();
                if (moveTimer >= moveTimeTotal)
                {
                    moving = false;
                }
                else
                {
                    if (movementType == 0)
                    {
                        transform.position = Vector3.Lerp(moveFromPoint, moveToPoint.position, moveTimer / moveTimeTotal);
                    }
                    else if (movementType == 1)
                    {
                        Vector3 newPosition = Vector3.MoveTowards(transform.position, moveToPoint.position, .2f);
                        //Vector3 newPosition = Vector3.Lerp(moveFromPoint, moveToPoint.position, moveTimer / moveTimeTotal);
                        if(lockY) newPosition.y = moveFromPoint.y;
						if(lockX) newPosition.x = moveFromPoint.x;
                        transform.position = newPosition;
                    }
                }
            }
        }
    }
}