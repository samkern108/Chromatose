using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public interface INotifyOnDeathObserver
    {
        void NotifyOnDeath(bool progressImmediate);
    }

    public class EnemyHealth : MonoBehaviour
    {
        public int healthMax = 3;
        private int currentHealth;
        public GameObject deathExplosion;

        private Animate animate;

        private Color baseColor, hitColor;

        public bool notifyOnDeath = false;
        public bool progressImmediate = false;

        public INotifyOnDeathObserver notifyDelegate;

        public void Start()
        {
            animate = GetComponent<Animate>();
            currentHealth = healthMax;

            baseColor = Color.white;
            hitColor = Color.red;
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            Debug.Log("On Trigger Enter");
            if (col.gameObject.tag == "Player" && PlayerController.dashing)
            {
                currentHealth--;
                animate.AnimateToColor(baseColor, hitColor, Level.secondsPerBeat, RepeatMode.OnceAndBack);
                Vector3 currentSize = transform.localScale;
                animate.AnimateToSize(currentSize, (currentSize - currentSize * .2f), Level.secondsPerBeat, RepeatMode.Once);
                if (currentHealth <= 0)
                {
                    Die();
                }
            }
            else if (!PlayerController.invulnerable)
            {
                PlayerController.self.Die();
            }
        }

        public void Die()
        {
            AudioManager.PlayEnemyDeath();
            animate.AnimateToColor(baseColor, hitColor, Level.secondsPerBeat * .05f, RepeatMode.PingPong);
            Vector3 currentSize = transform.localScale;
            animate.AnimateToSize(currentSize, (currentSize - currentSize * .2f), Level.secondsPerBeat * 2.0f, RepeatMode.Once);
            Invoke("Destroy", Level.secondsPerBeat * 2.0f);
        }

        private void Destroy()
        {
            Instantiate(deathExplosion, transform.position, Quaternion.identity);
            if (notifyOnDeath)
            {
                notifyDelegate.NotifyOnDeath(progressImmediate);
            }
            GameObject.Destroy(this.gameObject);
        }
    }
}