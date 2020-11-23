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
        
        if (StageManager.ourInstance.IsPositionInGrid(newPosition) 
            && !StageManager.ourInstance.CanEntityMoveToPosition(this, newPosition))
        {
            Entity entity = StageManager.ourInstance.GetEntity(newPosition);
            if (entity is Player)
            {
                entity.Kill(DeathReason.Enemy);
                hasPerformedAction = false;
                aTurnEvent.SignalDone();
                return;
            }

            TurnAround(aTurnEvent);
            return;
        }

        if (!StageManager.ourInstance.IsPositionInGrid(newPosition))
        {
            TurnAround(aTurnEvent);
            return;
        }

        Move(myDirection);
        hasPerformedAction = false;
        aTurnEvent.SignalDone();
        return;
    }

    private void TurnAround(TurnEvent aTurnEvent)
    {
        myDirection = ReverseDirection(myDirection);
        if (!hasPerformedAction)
        {
            hasPerformedAction = true;
            Action(aTurnEvent);
        }
        hasPerformedAction = false;
        aTurnEvent.SignalDone();
        return;
    }
}