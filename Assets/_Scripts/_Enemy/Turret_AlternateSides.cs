using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

namespace Chromatose
{
    public class Turret_AlternateSides : Turret
    {
        protected override void ShootMissile()
        {
            int shootSide = -1;
            RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, 40.0f, 1 << LayerMask.NameToLayer("Wall"));
            RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, 40.0f, 1 << LayerMask.NameToLayer("Wall"));

            //  Shoot to the left
            if (rightHit.distance < leftHit.distance)
            {
                shootSide = 1;
            }

            GameObject missile;
            Vector3 direction;
            float angle = shootAngle;
            for (int i = 0; i < 5; i++)
            {
                missile = Instantiate(projectile);//, ProjectileManager.myTransform);
                missile.transform.position = transform.position;
                direction = Vector3.up.Rotate2D(angle);
                angle += 45 * shootSide;
                missile.GetComponent<Projectile>().Initialize(direction, projectileSpeed, splitCount);
            }
            startSize = transform.localScale;
            bigSize = startSize + (startSize * .2f);
            animate.AnimateToSize(startSize, bigSize, .05f, RepeatMode.OnceAndBack);
        }
    }
}