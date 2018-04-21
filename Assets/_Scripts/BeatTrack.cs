using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using System.Linq;

namespace Chromatose
{
    public interface IMovementObserver
    {
        void StartMoving();
        void StopMoving();
    }
    public class BeatTrack : MonoBehaviour
    {
        public IMovementObserver movementObserver;

        [EventID]
        public string eventID;
        private Koreography koreo;

        private bool moving = false;
        private float moveTimeTotal = 0.0f;
        private float moveTimer = 0.0f;

        private Transform[] points;
        private int pointIndex = 0;

        public Transform objectToMove;
        private int samplesPerBeat;

        public int startPoint = 0;
        public bool snapToStartPoint = true;

        void Start()
        {
            Koreographer.Instance.RegisterForEventsWithTime(eventID, TrackMoveEvent);
            points = transform.GetComponentsInChildren<Transform>();
            points = points.Skip(1).ToArray();
            pointIndex = startPoint;

            samplesPerBeat = (int)Level.koreo.GetSamplesPerBeat(0);

            movementObserver = objectToMove.GetComponentInChildren<IMovementObserver>();

            if (snapToStartPoint)
            {
                objectToMove.position = points[pointIndex].position;
            }

            moveFromPoint = objectToMove.position;
            moveToPoint = points[pointIndex].position;

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
            CheckForDestroy();
            if (currentSampleTime != evt.StartSample)
            {
                currentSampleTime = evt.StartSample;
                moving = true;
                moveTimer = 0.0f;
                moveTimeTotal = ((evt.EndSample - evt.StartSample) / samplesPerBeat);

                movementObserver.StartMoving();
            }
        }

        private Vector3 moveFromPoint, moveToPoint;

        public void Update()
        {
            CheckForDestroy();
            if (moving)
            {
                moveTimer += koreo.GetBeatTimeDelta();
                if (moveTimer >= moveTimeTotal)
                {
                    moving = false;
                    movementObserver.StopMoving();
                    pointIndex = (pointIndex + 1) % points.Length;
                    moveFromPoint = points[pointIndex].position;
                    moveToPoint = points[(pointIndex + 1) % points.Length].position;
                }
                else
                {
                    objectToMove.position = Vector3.Lerp(moveFromPoint, moveToPoint, moveTimer / moveTimeTotal);
                }
            }
        }

        private void CheckForDestroy()
        {
            if (!objectToMove)
            {
                Destroy(this);
            }
        }
    }
}