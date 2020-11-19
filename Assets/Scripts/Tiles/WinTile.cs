using UnityEngine;

public class WinTile : Tile
{
    public override void OnEnter(Entity steppedOnMe)
    {
        if (steppedOnMe as Player != null)
        {
            GameManager.ourInstance.TransitionToNextStage();
        }
        base.OnEnter(steppedOnMe);
    }
}
