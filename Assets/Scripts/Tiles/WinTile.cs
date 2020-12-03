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
        if (steppedOnMe is Player)
        {
            StageManager.ourInstance.OnPlayerWon();
        }

        base.OnEnter(steppedOnMe);
    }
}
