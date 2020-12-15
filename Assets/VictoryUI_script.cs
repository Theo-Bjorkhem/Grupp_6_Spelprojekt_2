using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class VictoryUI_script : MonoBehaviour
{
    [SerializeField]
    GameObject myCredits;
    [SerializeField]
    GameObject myVictoryScreen;

    public void DisplayVictoryScreen()
    {
        if (!SceneManager.GetSceneByName("Victory_scene").isLoaded)
        {
            Time.timeScale = 0;
            SceneManager.GetSceneByName("HUD1_scene").GetRootGameObjects()[0].SetActive(false);
            if (AudioManager.ourInstance != null)
            {
                AudioManager.ourInstance.PlaySound("Pause");
            }
            SceneManager.LoadScene("Victory_scene", LoadSceneMode.Additive);
        }
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
    public void RestartGame()
    {
        GameManager.ourInstance.TransitionToStage(0);
        Time.timeScale = 1;
    }
    public void DisplayCredits()
    {
        myVictoryScreen.SetActive(false);
        myCredits.SetActive(true);
    }
    public void ReturnToMenu()
    {
        myVictoryScreen.SetActive(true);
        myCredits.SetActive(false);
    }
    public void QuitGame()
    {
        GameManager.ourInstance.Quit();
    }

}
