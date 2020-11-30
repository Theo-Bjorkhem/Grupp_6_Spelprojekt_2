﻿using System.Collections;
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
            AudioManager.ourInstance.PlaySound("Pause");
            SceneManager.LoadScene("PausMenu1_scene", LoadSceneMode.Additive);
        }
    }
    public void UnPasueGame()
    {
        Destroy(SceneManager.GetSceneByName("PausMenu1_scene").GetRootGameObjects()[0]);
        AudioManager.ourInstance.PlaySound("Unpause");
        SceneManager.UnloadSceneAsync("PausMenu1_scene");
        Time.timeScale = 1;
    }
    public void LoadMainMenu()
    {
        AudioManager.ourInstance.PlaySound("MenuNegative");
        SceneManager.LoadScene("mainMenu_scene");
        Time.timeScale = 1;
    }

    public void RestartLevel()
    {
        int i = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(i);
        Time.timeScale = 1;
    }
}
