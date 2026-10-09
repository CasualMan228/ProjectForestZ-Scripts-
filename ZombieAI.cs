using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTarget;

    public GameObject[] groanSoundPrefabs;
    public float minSoundDelay = 4.0f;
    public float maxSoundDelay = 8.0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        //находим игрока на сцене по тегу один раз при появлении зомби
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
        }

        //ОПТИМИЗАЦИЯ: Вместо тяжелого Update, заставляем зомби обновлять маршрут 
        //раз в 0.2 секунды. Для глаза это незаметно, но экономит кучу ресурсов CPU!
        InvokeRepeating("UpdatePath", 0.0f, 0.2f);

        StartCoroutine(PlayAmbientSoundsRoutine());
    }

    private void UpdatePath()
    {
        //проверяем, жив ли игрок и стоит ли агент на NavMesh сетке
        if (agent != null && playerTarget != null && agent.isOnNavMesh)
        {
            agent.SetDestination(playerTarget.position);
        }
    }

    private IEnumerator PlayAmbientSoundsRoutine()
    {
        //бесконечный цикл внутри жизни одного конкретного зомби
        while (true)
        {
            //1) выбираем случайное время захода на следующий рык
            float randomDelay = Random.Range(minSoundDelay, maxSoundDelay);
            yield return new WaitForSeconds(randomDelay);

            //2) проверяем, загружены ли префабы звуков
            if (groanSoundPrefabs != null && groanSoundPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, groanSoundPrefabs.Length);
                GameObject selectedSoundPrefab = groanSoundPrefabs[randomIndex];

                if (selectedSoundPrefab != null)
                {
                    //ПОД КАПОТОМ ->Четвертый аргумент 'transform' указывает движку, 
                    //что создаваемый объект звука обязан стать дочерним (Child) для этого зомби!
                    GameObject soundClone = Instantiate(selectedSoundPrefab, transform.position, Quaternion.identity, transform);

                    AudioSource audioSource = soundClone.GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        audioSource.Play();
                    }

                    //на всякий случай чистим память через 4 секунды, если зомби выживет,
                    //чтобы отработавшие звуки не копились внутри зомби
                    Destroy(soundClone, 4.0f);
                }
            }
        }
    }
}
