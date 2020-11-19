using UnityEngine;

public class WinTile : Tile
{
    protected override void Start()
    {
        base.Start();

        StageManager.ourInstance.RegisterTileForTurnEvents(this);
    }

    public override void OnEnter(Entity steppedOnMe)
    {
        if (steppedOnMe as Player != null)
        {
            if (GameManager.ourInstance == null)
            {
                Debug.LogWarning("Cannot transition to next stage because GameManager is null. Did you start the game from the main menu?", this);
            }
            else
            {
                StageManager.ourInstance.OnPlayerWon();
                GameManager.ourInstance.TransitionToNextStage();
            }
        }
        base.OnEnter(steppedOnMe);
    }
}
