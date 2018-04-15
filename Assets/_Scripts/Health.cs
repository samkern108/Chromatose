using UnityEngine;
using System.Collections;

namespace Chromatose
{
    public class Health : MonoBehaviour
    {

        public GameObject deathExplosion;

        public void OnTriggerEnter2D(Collider2D coll)
        {
            if (!PlayerController.dashing && !PlayerController.invulnerable)
            {
                Die();
            }
        }

        void Die()
        {
            NotificationMaster.SendPlayerDeathNotification();
            AudioManager.PlayPlayerDeath();
            Instantiate(deathExplosion, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Level.self.PlayerRespawn();
        }
    }
}