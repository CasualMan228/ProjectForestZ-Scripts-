using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    public string gameOverSceneName = "GameOverScene";

    private void OnTriggerEnter(Collider other)
    {
        //проверяем тег объекта, который вошел в нашу зону триггера
        if (other.CompareTag("Zombie"))
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}
