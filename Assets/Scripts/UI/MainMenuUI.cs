using UnityEngine;

public class MainMenuUI : MonoBehaviour
{

    public void OnTestButtonClicked()
    {
        GameManager.ourInstance.TransitionToStage(0);
    }

}
