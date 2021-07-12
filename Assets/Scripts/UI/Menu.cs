using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] Animator FadeOut;

    public void LoadLevelByName(string lvlName)
    {
        StartCoroutine(FO(lvlName));
    }

    //Fade Out and load level
    IEnumerator FO(string lvlName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(lvlName);
        asyncLoad.allowSceneActivation = false;
        FadeOut.SetBool("Fade",true);
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        asyncLoad.allowSceneActivation = true;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
