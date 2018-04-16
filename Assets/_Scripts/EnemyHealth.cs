using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public GameObject deathExplosion;
    public void Die()
    {
        AudioManager.PlayEnemyDeath();
        Instantiate(deathExplosion, transform.position, Quaternion.identity);
        GameObject.Destroy(this.gameObject);
    }
}
