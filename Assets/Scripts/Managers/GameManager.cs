using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager ourInstance;

    private int myCurrentStageIndex = -1; // -1 = not in stage

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
        if (myCurrentStageIndex >= 0)
        {
            TransitionToStage(myCurrentStageIndex + 1);
        }
        else
        {
            TransitionToMainMenu();
        }
    }

    private IEnumerator TransitionToMainMenuCo()
    {
        yield return TransitionToScene("MainMenu");

        myCurrentStageIndex = -1;
    }

    private IEnumerator TransitionToStageCo(int aStageIndex)
    {
        string stageSceneName = $"Stage{aStageIndex}";

        yield return TransitionToScene(stageSceneName);

        // Ensure the scene was loaded before continuing
        Scene scene = SceneManager.GetSceneByName(stageSceneName);
        if (scene == null)
        {
            Debug.LogError($"Failed to load scene {stageSceneName}!");
            yield break;
        }

        // TODO: Add UI scenes that need to be loaded on stage load here!
        // SceneManager.LoadScene("UIBase", LoadSceneMode.Additive);

        myCurrentStageIndex = aStageIndex;

        Debug.Assert(StageManager.ourInstance != null, "No StageManager in loaded stage!");
    }

    private IEnumerator TransitionToScene(string aSceneName)
    {
        // TODO: Fade out?

        // TODO: Start loading animation?

        yield return SceneManager.LoadSceneAsync(aSceneName);

        // TODO: Stop loading animation?

        // TODO: Fade in?
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
    }
}
