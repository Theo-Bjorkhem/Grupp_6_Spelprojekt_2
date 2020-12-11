using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_script : MonoBehaviour
{
    public void PauseGame()
    {
        if (!SceneManager.GetSceneByName("PausMenu1_scene").isLoaded)
        {
            Time.timeScale = 0;
            //SceneManager.UnloadSceneAsync("HUD1_scene");
            //Destroy(SceneManager.GetSceneByName("HUD1_scene").GetRootGameObjects()[0]);
            SceneManager.GetSceneByName("HUD1_scene").GetRootGameObjects()[0].SetActive(false);

            if (AudioManager.ourInstance != null)
            {
                AudioManager.ourInstance.PlaySound("Pause");
            }
            SceneManager.LoadScene("PausMenu1_scene", LoadSceneMode.Additive);
        }
    }
    public void UnPasueGame()
    {
        Destroy(SceneManager.GetSceneByName("PausMenu1_scene").GetRootGameObjects()[0]);
        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("Unpause");
        }
        SceneManager.UnloadSceneAsync("PausMenu1_scene");

        //SceneManager.LoadScene("HUD1_scene", LoadSceneMode.Additive);
        SceneManager.GetSceneByName("HUD1_scene").GetRootGameObjects()[0].SetActive(true);

        Time.timeScale = 1;
    }
    public void LoadMainMenu()
    {
        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("MenuNegative");
        }
        SceneManager.LoadScene("mainMenu_scene");
        Time.timeScale = 1;
    }
    public void RestartLevel()
    {
        GameManager.ourInstance.TransitionToStage(GameManager.ourInstance.GetStageIndex());
        Time.timeScale = 1;
    }
    public void OpenHowToPlay()
    {
        if (!SceneManager.GetSceneByName("Tutorial_scene").isLoaded)
        {
            SceneManager.LoadScene("Tutorial_scene", LoadSceneMode.Additive);
        }
    }
    public void CloseHowToPlay()
    {
        if (SceneManager.GetSceneByName("Tutorial_scene").isLoaded)
        {
            Destroy(SceneManager.GetSceneByName("Tutorial_scene").GetRootGameObjects()[0]);
            SceneManager.UnloadSceneAsync("Tutorial_scene");
        }
    }
}
