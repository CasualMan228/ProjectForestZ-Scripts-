using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsSounds : MonoBehaviour
{
    public GameObject[] soundsStepForest;
    public float stepInterval = 0.6f;
    private float holdTime = 0f;
    public void ProcessSteps(Vector3 spawnPosition)
    {
        holdTime += Time.deltaTime;

        //если зажали кнопку движения и прошло нужное время для шага
        if (holdTime >= stepInterval)
        {
            holdTime = 0f;

            if (soundsStepForest != null && soundsStepForest.Length > 0)
            {
                //выбираем рандомный звук из массива
                GameObject randomStepPrefab = soundsStepForest[Random.Range(0, soundsStepForest.Length)];

                //спавним префаб звука в точке игрока
                GameObject soundClone = Instantiate(randomStepPrefab, spawnPosition, Quaternion.identity);

                AudioSource currentSound = soundClone.GetComponent<AudioSource>();
                if (currentSound != null)
                {
                    currentSound.Play();
                }

                //удаляем клон звука через 10 секунд
                Destroy(soundClone, 10f);
            }
        }
    }
    public void ResetTimer()
    {
        holdTime = 0f;
    }
}
