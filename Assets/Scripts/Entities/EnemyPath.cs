using UnityEngine;

public class EnemyPath : Entity
{
    [SerializeField]
    private Transform myRoot = null;
    [SerializeField]
    private Direction[] mySteps;
    [SerializeField]
    private bool myWillReverse = true;

    private int myStepsIndex;

    public override void Action(TurnEvent aTurnEvent)
    {
        if (mySteps.Length <= 0)
        {
            aTurnEvent.SignalDone();
            return;
        }

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

        Tile tile = StageManager.ourInstance.GetTile(newPosition);
        if (tile is HoleTile holeTile && !holeTile.myIsFilled)
        {
            aTurnEvent.SignalDone();
            return;
        }

        if (Move(mySteps[myStepsIndex]))
        {
            switch (mySteps[myStepsIndex])
            {
                case Direction.Up:
                    //myRoot.localRotation.eulerAngles.Set(0, 0, 0);
                    myRoot.localEulerAngles = new Vector3(0f, 0f, 0f);
                    //myRoot.rotation.eulerAngles.Set(0, 0, 0);
                    break;
                case Direction.Right:
                    myRoot.localEulerAngles = new Vector3(0f, 90f, 0f);
                    //myRoot.rotation.eulerAngles.Set(0, 90, 0);
                    break;
                case Direction.Down:
                    //myRoot.localRotation.eulerAngles.Set(0, 180, 0);
                    myRoot.localEulerAngles = new Vector3(0f, 180f, 0f);
                    //myRoot.rotation.eulerAngles.Set(0, 180, 0);
                    break;
                case Direction.Left:
                    //myRoot.localRotation.eulerAngles.Set(0, 270, 0);
                    myRoot.localEulerAngles = new Vector3(0f, 270f, 0f);
                    //myRoot.rotation.eulerAngles.Set(0, 270, 0);
                    break;
                default:
                    Debug.LogError("Enemy: " + this + " can't rotate");
                    break;
            }
        }

        myStepsIndex++;

        if (myStepsIndex >= mySteps.Length)
        {
            myStepsIndex = 0;
            if (myWillReverse)
            {
                ReverseSteps();
            }
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