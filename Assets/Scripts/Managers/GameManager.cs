using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager ourInstance;

    public bool myIsInStage => myCurrentStageIndex >= 0;

    private int myCurrentStageIndex = -1; // -1 = not in stage

    public void UpdateStageScore(/* TODO: Some stage information structure */)
    {
        Debug.Assert(myIsInStage, "UpdateStageScore called while not in stage!");

        // TODO: Implement when we know how to score a stage etc.
    }

    public int GetStageIndex()
    {
        return myCurrentStageIndex;
    }

    public void TransitionToMainMenu()
    {
        StartCoroutine(TransitionToMainMenuCo());
    }

    public bool TransitionToStage(int aStageIndex)
    {
        // TODO: Check if stage unlocked etc..

        StartCoroutine(TransitionToStageCo(aStageIndex));

        return true;
    }


    public void TransitionToNextStage()
    {
        // TODO: Check that the next stage exists.
        if (myCurrentStageIndex + 1 < 12)
        {

            if (PlayerPrefs.GetInt("levelAt") < myCurrentStageIndex + 1)
            {
                PlayerPrefs.SetInt("levelAt", myCurrentStageIndex + 1);
            }
            TransitionToStage(myCurrentStageIndex + 1);
        }
        else
        {

            if (!SceneManager.GetSceneByName("Victory_scene").isLoaded)
            {
                Time.timeScale = 0;
                //SceneManager.UnloadSceneAsync("HUD1_scene");
                //Destroy(SceneManager.GetSceneByName("HUD1_scene").GetRootGameObjects()[0]);
                SceneManager.GetSceneByName("HUD1_scene").GetRootGameObjects()[0].SetActive(false);

                if (AudioManager.ourInstance != null)
                {
                    AudioManager.ourInstance.PlaySound("Pause");
                }
                SceneManager.LoadScene("Victory_scene", LoadSceneMode.Additive);
            }
        }
    }

    private IEnumerator TransitionToMainMenuCo()
    {
        yield return TransitionToScene("MainMenu");

        myCurrentStageIndex = -1;
    }

    private IEnumerator TransitionToStageCo(int aStageIndex)
    {
        string stageSceneName = $"Stage{aStageIndex + 1}_scene";

        yield return TransitionToScene(stageSceneName);

        // Ensure the scene was loaded before continuing
        Scene scene = SceneManager.GetSceneByName(stageSceneName);
        if (scene == null)
        {
            Debug.LogError($"Failed to load scene {stageSceneName}!");
            yield break;
        }

        // TODO: Add UI scenes that need to be loaded on stage load here!
        SceneManager.LoadScene("uiBase_scene", LoadSceneMode.Additive);
        SceneManager.LoadScene("HUD1_scene", LoadSceneMode.Additive);

        myCurrentStageIndex = aStageIndex;

        if (AudioManager.ourInstance != null)
        {
            if (myCurrentStageIndex == -1)
            {
                AudioManager.ourInstance.PlayLoop("MusicMainMenu");
                AudioManager.ourInstance.Stop("Music");
                AudioManager.ourInstance.Stop("Music2");
                AudioManager.ourInstance.Stop("Music3");
                AudioManager.ourInstance.Stop("Ambience");
                AudioManager.ourInstance.Stop("Ambience2");
                AudioManager.ourInstance.Stop("Ambience3");
            }
            else if (myCurrentStageIndex >= 0 && myCurrentStageIndex <= 3)
            {
                AudioManager.ourInstance.Stop("MusicMainMenu");
                AudioManager.ourInstance.PlayLoop("Music");
                AudioManager.ourInstance.Stop("Music2");
                AudioManager.ourInstance.Stop("Music3");
                AudioManager.ourInstance.PlayLoop("Ambience");
                AudioManager.ourInstance.Stop("Ambience2");
                AudioManager.ourInstance.Stop("Ambience3");
            }
            else if (myCurrentStageIndex >= 4 && myCurrentStageIndex <= 7)
            {
                AudioManager.ourInstance.Stop("MusicMainMenu");
                AudioManager.ourInstance.Stop("Music");
                AudioManager.ourInstance.PlayLoop("Music2");
                AudioManager.ourInstance.Stop("Music3");
                AudioManager.ourInstance.Stop("Ambience");
                AudioManager.ourInstance.PlayLoop("Ambience2");
                AudioManager.ourInstance.Stop("Ambience3");
            }
            else if (myCurrentStageIndex >= 8 && myCurrentStageIndex <= 11)
            {
                AudioManager.ourInstance.Stop("MusicMainMenu");
                AudioManager.ourInstance.Stop("Music");
                AudioManager.ourInstance.Stop("Music2");
                AudioManager.ourInstance.PlayLoop("Music3");
                AudioManager.ourInstance.Stop("Ambience");
                AudioManager.ourInstance.Stop("Ambience2");
                AudioManager.ourInstance.PlayLoop("Ambience3");
            }
        }

        Debug.Assert(StageManager.ourInstance != null, "No StageManager in loaded stage!");
    }

    private IEnumerator TransitionToScene(string aSceneName)
    {
        yield return EffectUI.ourInstance.FadeOut(0.8f);

        // TODO: Start loading animation?

        yield return SceneManager.LoadSceneAsync(aSceneName);

        // TODO: Stop loading animation?

        yield return EffectUI.ourInstance.FadeIn(0.8f);
    }

    private void Awake()
    {
        if (ourInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        ourInstance = this;

        SceneManager.LoadScene("effectUi_scene", LoadSceneMode.Additive);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
