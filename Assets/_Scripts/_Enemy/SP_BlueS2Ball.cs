using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class SP_BlueS2Ball : StagePiece
    {
        public void StageCompleted()
        {
            Turret t = GetComponentInChildren<Turret>();
            if (t) t.enabled = true;
            IncreaseSpeed i = GetComponentInChildren<IncreaseSpeed>();
            if (i) i.enabled = false;
        }
    }
}