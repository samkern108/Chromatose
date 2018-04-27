using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class Blue_StageFive : MonoBehaviour
    {
		public Track track;
        public void LoadingStage()
        {
            Turret[] turrets = GetComponentsInChildren<Turret>();
			foreach(Turret t in turrets) {
				t.shootAngle = 45;
				t.splitCount = 2;
			}

			BeatTrack[] beatTracks = GetComponentsInChildren<BeatTrack>();
			foreach(BeatTrack bt in beatTracks) {
				bt.track = track;
			}
        }
    }
}