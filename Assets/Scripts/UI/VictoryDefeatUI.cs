using UnityEngine;

public class VictoryDefeatUI : MonoBehaviour
{
    /// <summary>
    /// Called to show the victory / defeat screen.
    /// </summary>
    public void Show(bool anIsVictory)
    {
        gameObject.SetActive(true);

        // TODO: Implement...
    }

    public void OnMainMenuClicked()
    {
        GameManager.ourInstance.TransitionToMainMenu();
    }

    private void OnPlayerWon() => Show(true);

    private void OnPlayerDefeated() => Show(false);

    private void Start()
    {
        StageManager.ourInstance.myStageMessages.myOnPlayerWon += OnPlayerWon;
        StageManager.ourInstance.myStageMessages.myOnPlayerDefeated += OnPlayerDefeated;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (StageManager.ourInstance == null)
        {
            return;
        }

        StageManager.ourInstance.myStageMessages.myOnPlayerDefeated -= OnPlayerDefeated;
        StageManager.ourInstance.myStageMessages.myOnPlayerWon -= OnPlayerWon;
    }
}
