using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public int health = 1;

    public GameObject deathParticlesPrefab;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(10);
        }

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnZombieDeath();
        }

        if (deathParticlesPrefab != null)
        {
            GameObject particlesClone = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);

            Destroy(particlesClone, 3.0f);
        }

        Destroy(gameObject);
    }
}
