using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class Turret_BL_S2 : Turret
    {
        protected override void ShootMissile()
        {
            GameObject missile;
            Vector3 direction;
            float angle = shootAngle;
            for (int i = 0; i < 4; i++)
            {
                missile = Instantiate(projectile);//, ProjectileManager.myTransform);
                missile.transform.position = transform.position;
                direction = Vector3.up.Rotate2D(angle);
                angle += 90;
                missile.GetComponent<Projectile>().Initialize(direction, projectileSpeed, splitCount);
            }
            //shootAngle -= 45;
            startSize = transform.localScale;
            bigSize = startSize + (startSize * .2f);
            animate.AnimateToSize(startSize, bigSize, .05f, RepeatMode.OnceAndBack);
        }
    }
}