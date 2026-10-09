using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public Text recordText;
    public NewBehaviourScript playerControllerScript;
    public GameObject menuMusic;
    private GameObject activeMenuMusic;
    public GameObject clickSound;
    public GameObject gunLoadedSound;
    public GameObject weaponHolder;

    public GameObject gamePlayCanvas;

    public GameObject gameMusic;
    private GameObject activeGameMusic;

    private void Start()
    {
        if (weaponHolder != null)
        {
            weaponHolder.SetActive(false);
        }

        if (gamePlayCanvas != null)
        {
            gamePlayCanvas.SetActive(false);
        }

        //1) показываем меню при старте сцены
        menuCanvas.SetActive(true);

        //2) отключаем скрипт движени€ игрока, чтобы он не ходил и не крутил камерой сквозь меню
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = false;
        }

        //3) высвобождаем курсор мыши дл€ кликов по кнопкам
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //4) загружаем и выводим рекорд из пам€ти
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (recordText != null)
        {
            recordText.text = "–екорд: " + highScore.ToString();
        }

        if (menuMusic != null)
        {
            activeMenuMusic = Instantiate(menuMusic, Vector3.zero, Quaternion.identity);
            AudioSource audio = activeMenuMusic.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Play();
            }
        }
    }
    public void PlayGame()
    {
        SpawnClickSound();
        if (activeMenuMusic != null)
        {
            Destroy(activeMenuMusic);
        }
        if (gunLoadedSound != null)
        {
            GameObject startSoundClone = Instantiate(gunLoadedSound, Vector3.zero, Quaternion.identity);
            AudioSource audio = startSoundClone.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Play();
            }
            Destroy(startSoundClone, 5f);
        }

        if (gameMusic != null)
        {
            activeGameMusic = Instantiate(gameMusic, Vector3.zero, Quaternion.identity);
            AudioSource audio = activeGameMusic.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Play();
            }
        }

        if (weaponHolder != null)
        {
            weaponHolder.SetActive(true);

            //достаем компонент јниматора и принудительно запускаем анимацию по имени файла
            Animator weaponAnimator = weaponHolder.GetComponent<Animator>();
            if (weaponAnimator != null)
            {
                weaponAnimator.Play("LoadWeapon");
            }
        }

        if (gamePlayCanvas != null)
        {
            gamePlayCanvas.SetActive(true);
        }

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.StartFirstWave();
        }

        //1) пр€чем меню
        menuCanvas.SetActive(false);

        //2) включаем скрипт игрока
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true;
        }

        //3) лочим курсор в центре экрана дл€ игры
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void ExitGame()
    {
        SpawnClickSound();

        Application.Quit();
    }
    private void SpawnClickSound()
    {
        if (clickSound != null)
        {
            GameObject clickClone = Instantiate(clickSound, Vector3.zero, Quaternion.identity);
            AudioSource audio = clickClone.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Play();
            }
            Destroy(clickClone, 3f);
        }
    }
}
