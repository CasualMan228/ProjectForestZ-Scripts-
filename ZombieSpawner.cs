using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject fastZombieObject;
    public GameObject slowZombieObject;
    GameObject playerTarget;

    public GameObject spawnParticlesPrefab;
    public float effectDuration = 2.5f;

    //список для хранения всех живых зомби на сцене
    private List<NavMeshAgent> activeZombies = new List<NavMeshAgent>();

    private void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        SpawnAndTarget();
    }

    //каждый кадр заставляем всех зомби из списка бежать за актуальной позицией игрока
    private void Update()
    {
        if (playerTarget == null) return;

        //цикл перебирает каждого зомби, которого мы заспавнили
        for (int i = 0; i < activeZombies.Count; i++)
        {
            //зомби еще жив -> не убит или пропал?
            if (activeZombies[i] != null)
            {
                activeZombies[i].SetDestination(playerTarget.transform.position);
            }
        }
    }

    private void SpawnAndTarget()
    {
        Vector3 spawnPosition = Vector3.zero;

        //1) спавним быстрого зомби
        if (fastZombieObject != null)
        {
            GameObject fastZombie = Instantiate(fastZombieObject, spawnPosition, Quaternion.identity);
            NavMeshAgent agent = fastZombie.GetComponent<NavMeshAgent>();

            if (agent != null)
            {
                activeZombies.Add(agent);
            }

            SpawnParticleEffect(spawnPosition);
        }

        //2) спавним медленного зомби
        if (slowZombieObject != null)
        {
            GameObject slowZombie = Instantiate(slowZombieObject, spawnPosition, Quaternion.identity);
            NavMeshAgent agent = slowZombie.GetComponent<NavMeshAgent>();

            if (agent != null)
            {
                activeZombies.Add(agent);
            }

            SpawnParticleEffect(spawnPosition);
        }
    }

    private void SpawnParticleEffect(Vector3 position)
    {
        if (spawnParticlesPrefab != null)
        {
            GameObject effectClone = Instantiate(spawnParticlesPrefab, position, Quaternion.identity);
            Destroy(effectClone, effectDuration);
        }
    }
}
