using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Victory_script : MonoBehaviour
{
    [SerializeField]
    GameObject myCreditPanel;
    [SerializeField]
    GameObject myButtonPanel;

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
        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("MenuNegative");
        }
        GameManager.ourInstance.TransitionToStage(0);
        Time.timeScale = 1;
    }
    public void DisplayCredits()
    {
        myButtonPanel.SetActive(false);
        myCreditPanel.SetActive(true);
    }
    public void ReturnToButtons()
    {
        myButtonPanel.SetActive(true);
        myCreditPanel.SetActive(false);
    }
    public void QuitGame()
    {
        GameManager.ourInstance.Quit();
    }
}
