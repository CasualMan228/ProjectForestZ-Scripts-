using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public GameObject spawnParticlesPrefab;
    public float effectDuration = 2.5f;

    public static WaveManager Instance;

    public GameObject[] zombiePrefabs;
    public Transform playerTransform;
    public Text waveNotificationText;

    public float minSpawnDistance = 50f;
    public float maxSpawnDistance = 135f;

    private int currentWave = 0;
    private int zombiesRemainingToKill = 0;
    private bool isGameStarted = false;

    private void Awake()
    {
        Instance = this;
        if (waveNotificationText != null) waveNotificationText.enabled = false;
    }

    public void StartFirstWave()
    {
        isGameStarted = true;
        currentWave = 1;
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        //1) мигание текста ровно 3 раза
        if (waveNotificationText != null)
        {
            waveNotificationText.text = "Волна №" + currentWave;
            for (int i = 0; i < 3; i++)
            {
                waveNotificationText.enabled = true;
                yield return new WaitForSeconds(0.5f);
                waveNotificationText.enabled = false;
                yield return new WaitForSeconds(0.5f);
            }
        }

        //2) математические формулы под капотом волны
        //количество: на 1 волне (3-5), на 2 волне (3-6), на 3 волне (3-7) и т.д.
        int zombiesToSpawn = Random.Range(3, currentWave + 5);
        zombiesRemainingToKill = zombiesToSpawn;

        //множитель скорости: +15% к базовой скорости агента за каждую волну после первой
        float speedMultiplier = 1f + (currentWave - 1) * 0.15f;

        //3) спавн зомби
        for (int i = 0; i < zombiesToSpawn; i++)
        {
            SpawnZombie(speedMultiplier);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void SpawnZombie(float speedMultiplier)
    {
        if (zombiePrefabs.Length == 0 || playerTransform == null) return;

        //ищем случайную точку на круге вокруг игрока
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 randomPosition = new Vector3(playerTransform.position.x + randomCircle.x, playerTransform.position.y, playerTransform.position.z + randomCircle.y);

        //проверяем, пересекает ли эта точка NavMesh геометрию
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomPosition, out navHit, 10f, NavMesh.AllAreas))
        {
            //выбираем случайный префаб из твоих четырех видов
            int randomPrefabIndex = Random.Range(0, zombiePrefabs.Length);
            GameObject zombieClone = Instantiate(zombiePrefabs[randomPrefabIndex], navHit.position, Quaternion.identity);

            if (spawnParticlesPrefab != null)
            {
                GameObject effectClone = Instantiate(spawnParticlesPrefab, navHit.position, Quaternion.identity);
                Destroy(effectClone, effectDuration);
            }

            //настраиваем скорость NavMeshAgent
            NavMeshAgent agent = zombieClone.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed *= speedMultiplier;
            }

            //настраиваем прогрессию здоровья по твоей логике:
            //волна 1 -> здоровье строго 1. Волна 2 -> от 1 до 2. Волна 4 -> от 1 до 4.
            ZombieHealth healthScript = zombieClone.GetComponentInParent<ZombieHealth>();
            if (healthScript != null)
            {
                healthScript.health = Random.Range(1, currentWave + 1);
            }
        }
        else
        {
            //если точка не нашлась на NavMesh сетке, пробуем заспавнить еще раз в этом же кадре
            SpawnZombie(speedMultiplier);
        }
    }

    public void OnZombieDeath()
    {
        if (!isGameStarted) return;

        zombiesRemainingToKill--;

        //если уничтожены все зомби текущей волны -> запускаем следующую
        if (zombiesRemainingToKill <= 0)
        {
            currentWave++;
            StartCoroutine(SpawnWaveRoutine());
        }
    }
}
