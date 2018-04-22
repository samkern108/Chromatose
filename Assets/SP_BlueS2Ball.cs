using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class SP_BlueS2Ball : StagePiece
    {
		protected override void TransitionToNextStage() {
			EnemyHealth eh = GetComponentInChildren<EnemyHealth>();
			if(eh) eh.RestoreToFullHealth();
			Drone d = GetComponentInChildren<Drone>();
			if(d) d.enabled = true;
		}
    }
}