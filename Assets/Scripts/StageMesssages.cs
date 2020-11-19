using UnityEngine.Events;

public class StageMesssages
{
    public event UnityAction myOnPlayerWon;
    public event UnityAction myOnPlayerDefeated;

    public event UnityAction myOnTurnStart;

    public void TriggerPlayerWon()
    {
        myOnPlayerWon?.Invoke();
    }

    public void TriggerPlayerDefeated()
    {
        myOnPlayerDefeated?.Invoke();
    }

    public void TriggerTurnStart()
    {
        myOnTurnStart?.Invoke();
    }
}
