using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDebugCommands : MonoBehaviour
{
    [SerializeField] private int myFingersToGoBack = 2;
    [SerializeField] private int myFingersToReset = 3;
    [SerializeField] private int myFingersToGoForward = 4;
    [SerializeField] private float myHoldToResetDuration = 3f;
    private float myTimer = 0f;
    
    void Start()
    {
        // only enable in Development Builds
        enabled = Debug.isDebugBuild;
    }
    
    void Update()
    {
        if (Input.touchCount > 1)
        {
            myTimer += Time.deltaTime;
            if (myTimer > myHoldToResetDuration)
            {
                if (Input.touchCount == myFingersToGoBack)
                {
                    PreviousStage();
                }
                else if (Input.touchCount == myFingersToReset)
                {
                    ReloadScene();
                }
                else if (Input.touchCount == myFingersToGoForward)
                {
                    NextStage();
                }
            }
        }
        else
        {
            myTimer = 0f;
        }
        
        

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextStage();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PreviousStage();
        }
    }

    private void NextStage()
    {
        GameManager.ourInstance?.TransitionToNextStage();
    }

    private void PreviousStage()
    {
        GameManager.ourInstance?.TransitionToStage(
            SceneManager.GetActiveScene().buildIndex
            - SceneManager.GetSceneByName("MartinScene1_scene").buildIndex);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
