using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDebugCommands : MonoBehaviour
{
    [SerializeField] private int myFingersToHold = 3;
    [SerializeField] private float myHoldToResetDuration = 3f;
    private float myTimer = 0f;
    
    void Start()
    {
        // only enable in Development Builds
        enabled = Debug.isDebugBuild;
    }
    
    void Update()
    {
        if (Input.touchCount == myFingersToHold)
        {
            myTimer += Time.deltaTime;
            if (myTimer > myHoldToResetDuration)
            {
                ReloadScene();
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
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
