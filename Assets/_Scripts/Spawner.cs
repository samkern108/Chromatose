using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

namespace Chromatose
{
    public class Spawner : MonoBehaviour
    {
        public GameObject objectToSpawn;

        [EventID]
        public string eventID;
        private Koreography koreo;
        void Start()
        {
            Koreographer.Instance.RegisterForEventsWithTime(eventID, SpawnEvent);
        }

        void SpawnEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
        {
            GameObject spawned = GameObject.Instantiate(objectToSpawn);
            spawned.transform.position = BackgroundColor.GetRandomPointInRoom();
			spawned.GetComponent<DumbBall>().Spawn();
        }
    }
}