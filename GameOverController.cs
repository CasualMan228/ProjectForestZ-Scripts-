using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public Text totalRecordText;

    public string mainMenuSceneName = "SampleScene";
    public float delayBeforeReturn = 20f;

    private void Start()
    {
        //1) вытаскиваем из системного реестра/файла сохраненный рекорд
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);

        if (totalRecordText != null)
        {
            totalRecordText.text = "¬аш рекорд: " + savedHighScore.ToString();
        }

        //2) запускаем независимый таймер ожидани€ под капотом движка
        StartCoroutine(ReturnToMenuRoutine());
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuSceneName);

        //запрещаем сцене активироватьс€ самой, пока идет таймер ессс
        asyncLoad.allowSceneActivation = false;

        //движок засыпает в этой точке на твои 20 секунд. 
        //пока игрок грустит над рекордом, сцена меню тихо и плавно загружаетс€ в RAM
        yield return new WaitForSeconds(delayBeforeReturn);

        //3) врем€ вышло! —цена уже давно готова в пам€ти. ћгновенно врубаем еЄ!
        asyncLoad.allowSceneActivation = true;
    }
}
