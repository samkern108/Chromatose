﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class KillHeroOnContact : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D col)
        {
            HitPlayer();
        }

        public void OnCollisionEnter2D(Collision2D col)
        {
            HitPlayer();
        }

        private void HitPlayer()
        {
            if (!PlayerController.dashing && !PlayerController.invulnerable)
            {
                PlayerController.self.Hit();
            }
        }
    }
}