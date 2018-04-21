using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

namespace Chromatose
{
    public class MeteorSpawner : MonoBehaviour
    {
        public GameObject objectToSpawn;

        [EventID]
        public string eventID;
        private Koreography koreo;
        void Start()
        {
            Koreographer.Instance.RegisterForEventsWithTime(eventID, SpawnEvent);
        }

        void OnDestroy()
        {
            if (Koreographer.Instance != null)
                Koreographer.Instance.UnregisterForAllEvents(this);
        }

        void SpawnEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            GameObject spawned = GameObject.Instantiate(objectToSpawn);
			Vector3 newPosition = BackgroundColor.GetRandomPointInRoom();
			newPosition.y = BackgroundColor.bounds.max.y;
            spawned.transform.position = newPosition;
        }
    }
}