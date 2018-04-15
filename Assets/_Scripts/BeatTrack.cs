﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using System.Linq;

namespace Chromatose
{
    public class BeatTrack : MonoBehaviour
    {

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

        void Start()
        {
            Koreographer.Instance.RegisterForEventsWithTime(eventID, TrackMoveEvent);
            points = transform.GetComponentsInChildren<Transform>();
            points = points.Skip(1).ToArray();
            pointIndex = startPoint;

            samplesPerBeat = (int)Level.koreo.GetSamplesPerBeat(0);

            objectToMove.position = points[pointIndex].position;

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

                objectToMove.SendMessage("StartMoving");
            }
        }

        public void Update()
        {
            CheckForDestroy();
            if (moving)
            {
                moveTimer += koreo.GetBeatTimeDelta();
                if (moveTimer >= moveTimeTotal)
                {
                    moving = false;
                    objectToMove.SendMessage("StopMoving");
                    pointIndex = (pointIndex + 1) % points.Length;
                }
                else
                {
                    objectToMove.position = Vector3.Lerp(points[pointIndex].position, points[(pointIndex + 1) % points.Length].position, moveTimer / moveTimeTotal);
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