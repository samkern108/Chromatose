using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class StageConstants : MonoBehaviour
    {
		public static StageConstants self;
		public Color enemyInvuln1, enemyInvuln2, enemyHit1, enemyHit2, backgroundBase, backgroundLight, backgroundUltra;

		public void Start() {
			self = this;
		}
    }
}