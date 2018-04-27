using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class Blue_S4_EnemyBall : EnemyBall
    {
		private Turret _turret;
		void Awake() {
			base.Awake();
			_turret = GetComponent<Turret>();
		}

        public override void StartMoving()
        {
			base.StartMoving();
            _turret.turretActive = true;
        }

        public override void StopMoving()
        {
			base.StartMoving();
            _turret.turretActive = false;
        }
    }
}