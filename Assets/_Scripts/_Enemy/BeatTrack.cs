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
        public int payload = 0;
        public IMovementObserver movementObserver;

        [EventID]
        public string eventID;
        private Koreography koreo;

        private bool moving = false, canMove = false;
        private float moveTimeTotal = 0.0f;
        private float moveTimer = 0.0f;

        public Track track;
        private int moveToPointIndex = 0;

        private int samplesPerBeat;

        public int startPoint = 0;
        public bool snapToStartPoint = true;

        private bool firstMoveAfterStageBegin = true;

        void Start()
        {
            Koreographer.Instance.RegisterForEventsWithTime(eventID, TrackMoveEvent);

            moveToPointIndex = startPoint;
            if (snapToStartPoint)
            {
                transform.position = track.points[moveToPointIndex].position;
                moveToPointIndex++;
            }

            moveFromPoint = transform.position;
            moveToPoint = track.points[moveToPointIndex].position;

            movementObserver = GetComponentInChildren<IMovementObserver>();

            samplesPerBeat = (int)Level.koreo.GetSamplesPerBeat(0);
            koreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        }

        void OnDestroy()
        {
            if (Koreographer.Instance != null)
                Koreographer.Instance.UnregisterForAllEvents(this);
        }
        private KoreographyEvent currentEvent;
        void TrackMoveEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            int evtPayload = ((IntPayload)evt.Payload).IntVal;
            if(evtPayload != payload) return;
            // This breaks if we only have one track while looping
            if (canMove && (currentEvent == null || currentEvent.StartSample != evt.StartSample))
            {
                if (firstMoveAfterStageBegin)
                {
                    Invulnerable i = GetComponentInChildren<Invulnerable>();
                    if (i != null)
                        i.TurnOffInvulnerable();
                }

                currentEvent = evt;
                moving = true;
                moveTimer = 0.0f;
                moveTimeTotal = ((evt.EndSample - evt.StartSample) / samplesPerBeat);

                moveFromPoint = transform.position;
                moveToPoint = track.points[moveToPointIndex].position;
                moveToPointIndex = (moveToPointIndex + 1) % track.points.Length;

                movementObserver.StartMoving();
                firstMoveAfterStageBegin = false;
            }
        }

        private Vector3 moveFromPoint, moveToPoint;

        public void Update()
        {
            if (moving)
            {
                moveTimer += koreo.GetBeatTimeDelta();
                if (moveTimer >= moveTimeTotal)
                {
                    moving = false;
                    movementObserver.StopMoving();
                }
                else
                {
                    transform.position = Vector3.Lerp(moveFromPoint, moveToPoint, moveTimer / moveTimeTotal);
                }
            }
        }

        public void LoadingStage()
        {
            canMove = false;
            Invoke("StageBegin", Level.secondsPerMeasure * 2.0f);
        }

        public void StageBegin()
        {
            if (!canMove)
            {
                canMove = true;
                firstMoveAfterStageBegin = true;
            }
        }
    }
}