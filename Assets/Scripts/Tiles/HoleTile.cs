public class HoleTile : Tile
{
    public bool myIsFilled => myMoveableBox != null;

    private MoveableBox myMoveableBox;

    public override void OnEnter(Entity steppedOnMe)
    {
        base.OnEnter(steppedOnMe);

        if (myIsFilled)
            return;

        if (steppedOnMe is MoveableBox moveableBox)
        {
            moveableBox.OnFellInHole(this);
            myMoveableBox = moveableBox;
            return;
        }

        if (steppedOnMe is Player)
        {
            steppedOnMe.Kill(DeathReason.Fall);
        }
    }
}
