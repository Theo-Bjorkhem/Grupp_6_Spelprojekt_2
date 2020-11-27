using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_script : MonoBehaviour
{
    public void PauseGame()
    {
        Time.timeScale = 0;
        SceneManager.LoadScene("PausMenu1_scene", LoadSceneMode.Additive);
    }
    public void UnPasueGame()
    {
        Destroy(SceneManager.GetSceneByName("PausMenu1_scene").GetRootGameObjects()[0]);
        SceneManager.UnloadSceneAsync("PausMenu1_scene");
        Time.timeScale = 1;
    }
    public void LoadMainMenu()
    {

    }
}
