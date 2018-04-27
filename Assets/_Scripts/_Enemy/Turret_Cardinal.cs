using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class Turret_Cardinal : Turret
    {
        /*protected override void ShootMissile()
        {
            int shootAngle = 0;
            RaycastHit2D upHit = Physics2D.Raycast(transform.position, Vector2.up, 40.0f, 1 << LayerMask.NameToLayer("Wall"));
            RaycastHit2D downHit = Physics2D.Raycast(transform.position, Vector2.down, 40.0f, 1 << LayerMask.NameToLayer("Wall"));

            //  Shoot down
            if (upHit.distance > downHit.distance)
            {
                shootAngle = 180;
            }

            GameObject missile;
            Vector3 direction;

            missile = Instantiate(projectile);
            missile.transform.position = transform.position;
            direction = Vector3.down.Rotate2D(shootAngle);
            missile.GetComponent<Projectile>().Initialize(direction, projectileSpeed, splitCount);

            startSize = transform.localScale;
            bigSize = startSize + (startSize * .2f);
            animate.AnimateToSize(startSize, bigSize, .05f, RepeatMode.OnceAndBack);
        }*/

        protected override void ShootMissile()
        {
            GameObject missile;
            Vector3 direction;
            float angle = shootAngle;
            RaycastHit2D hit;

            Bounds bounds = BackgroundColor.bounds;
            float tooSmall = bounds.size.y / 4.0f;

            for (int i = 0; i < 360; i += 90)
            {
                hit = Physics2D.Raycast(transform.position, Vector2.up.Rotate2D(i + angle), tooSmall, 1 << LayerMask.NameToLayer("Wall"));
                if (!hit)
                {
                    missile = Instantiate(projectile);
                    missile.transform.position = transform.position;
                    direction = Vector3.up.Rotate2D(i + angle);
                    missile.GetComponent<Projectile>().Initialize(direction, projectileSpeed, splitCount);
                }
            }

            startSize = transform.localScale;
            bigSize = startSize + (startSize * .2f);
            animate.AnimateToSize(startSize, bigSize, .05f, RepeatMode.OnceAndBack);
        }
    }
}