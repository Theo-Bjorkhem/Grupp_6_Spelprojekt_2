using UnityEngine;

public class EnemyPatrol : Entity
{
    [SerializeField]
    private Direction myDirection;

    private bool hasPerformedAction = false;

    public override void Action(TurnEvent aTurnEvent)
    {
        Vector2Int currentPosition = StageManager.ourInstance.GetEntityGridPosition(this);
        Vector2Int newPosition = currentPosition;
        newPosition += DirectionToVec(myDirection);

        if (StageManager.ourInstance.CanEntityMoveToPosition(this, newPosition))
        {
            Move(myDirection);
            hasPerformedAction = false;
            aTurnEvent.SignalDone();
            return;
        }

        myDirection = ReverseDirection(myDirection);

        if (!hasPerformedAction)
        {
            hasPerformedAction = true;
            Action(aTurnEvent);
        }

        aTurnEvent.SignalDone();
    }

    public override void Interact(Entity anEntity, Direction aDirection)
    {
        if (anEntity is Player)
        {
            //kill player
        }
    }
}