using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class Projectile : MonoBehaviour
    {

        protected Vector3 moveVector;
        public GameObject p_explosion;
        private Animate animate;
        protected float speed;
        public Color color1 = Color.white, color2 = Color.white;

        private int splitCount = 0;
        private int splitsLeft;

        private Vector3 normalScale;

        public void Initialize(Vector3 direction, float speed, int splitCount)
        {
            this.speed = speed;
            moveVector = direction * speed;
            animate = GetComponent<Animate>();
            animate.AnimateToColor(color1, color2, Level.secondsPerBeat / 4.0f, RepeatMode.PingPong);
            this.splitCount = splitCount;
            splitsLeft = splitCount;
            normalScale = transform.localScale;
        }

        void Update()
        {
            transform.position += (moveVector * Time.deltaTime);
            transform.Rotate(new Vector3(0, 0, 200 * Time.deltaTime));
        }

        public void OnBecameInvisible()
        {
            Destroy(gameObject);
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (LayerMask.LayerToName(collider.gameObject.layer) == "Wall")
            {
                HitWall();
            }
            else if (LayerMask.LayerToName(collider.gameObject.layer) == "Player")
            {
                PlayerController.self.Hit();
                Destroy(gameObject);
            }
        }

        protected virtual void HitWall()
        {
            // Make sure we're including all things projectiles can hit
            RaycastHit2D hit = Physics2D.Raycast(transform.position, moveVector.normalized, 2f, 1 << LayerMask.NameToLayer("Wall"));
            Vector2 reflection = Vector2.Reflect(moveVector.normalized, hit.normal);

            if (hit.collider)
            {
                splitsLeft--;
                if (splitsLeft > 0)
                {
                    transform.localScale = (normalScale + normalScale * ((float)(splitsLeft + 1) / (float)(splitCount + 1))) / 2.0f;
                    moveVector = reflection * speed;
                }
                else
                {
                    GameObject explosion = Instantiate(p_explosion);

                    float size = GetComponent<Collider2D>().bounds.size.x;

                    explosion.transform.position = (Vector3)hit.point - (moveVector.normalized * size / 2.0f);
                    explosion.transform.rotation = Quaternion.LookRotation(reflection, Vector3.up);
                    Destroy(gameObject);
                }
            }
        }
    }
}