using UnityEngine;

public class EnemySeek : Entity
{
    [SerializeField]
    private Direction[] mySteps;

    private int myStepsIndex;

    public override void Action(TurnEvent aTurnEvent)
    {
        Vector2Int newPosition = StageManager.ourInstance.GetEntityGridPosition(this) + DirectionToVec(mySteps[myStepsIndex]);

        if (!StageManager.ourInstance.IsPositionInGrid(newPosition))
        {
            aTurnEvent.SignalDone();
            return;
        }

        if (!StageManager.ourInstance.CanEntityMoveToPosition(this, newPosition))
        {
            Entity entity = StageManager.ourInstance.GetEntity(newPosition);

            if (entity is Player)
            {
                entity.Kill(DeathReason.Enemy);
            }

            aTurnEvent.SignalDone();
            return;
        }

        Move(mySteps[myStepsIndex]);
        myStepsIndex++;

        if (myStepsIndex >= mySteps.Length)
        {
            myStepsIndex = 0;
            ReverseSteps();
        }

        aTurnEvent.SignalDone();
    }

    private void ReverseSteps()
    {
        Direction[] newSteps = new Direction[mySteps.Length];

        for (int i = mySteps.Length - 1; i >= 0; i--)
        {
            Direction direction = ReverseDirection(mySteps[i]);
            newSteps[mySteps.Length - 1 - i] = direction;
        }

        mySteps = newSteps;
    }
}