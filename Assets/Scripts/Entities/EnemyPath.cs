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
    private Vector3 myLerpStart = Vector3.zero;
    private Vector3 myLerpTarget = Vector3.zero;
    private float myLerpT = 0.0f;
    private bool myIsLerping = false;

    private VisualIndicators.IndicatorData myIndicatorData;

    private void Update()
    {
        // Do something ugly just to make it work
        if (myIndicatorData == null && !IsDead())
        {
            myIndicatorData = StageManager.ourInstance.myVisualIndicators.CreateNextStepIndicator();
            UpdateStepIndicator();
        }

        if (!myIsLerping)
        {
            return;
        }

        myLerpT += Time.deltaTime * 2;
        if (myLerpT >= 1)
        {
            myLerpT = 0;
            myIsLerping = false;
            transform.position = myLerpTarget;
            myLerpTarget = Vector3.zero;
            myLerpStart = Vector3.zero;
            return;
        }

        Vector3 position = Vector3.Lerp(myLerpStart, myLerpTarget, myLerpT);
        transform.position = position;
    }

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
                    myRoot.localEulerAngles = new Vector3(0f, 0f, 0f);
                    break;
                case Direction.Right:
                    myRoot.localEulerAngles = new Vector3(0f, 90f, 0f);
                    break;
                case Direction.Down:
                    myRoot.localEulerAngles = new Vector3(0f, 180f, 0f);
                    break;
                case Direction.Left:
                    myRoot.localEulerAngles = new Vector3(0f, 270f, 0f);
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

        UpdateStepIndicator();

        aTurnEvent.SignalDone();
    }

    public override void Kill(DeathReason aReason)
    {
        base.Kill(aReason);

        if (myIndicatorData != null)
        {
            StageManager.ourInstance.myVisualIndicators.RemoveIndicator(myIndicatorData);
            myIndicatorData = null;
        }
    }

    private void UpdateStepIndicator()
    {
        Vector2Int nextPosition = Vector2Int.zero;

        if (ComputeCurrentStep(ref nextPosition))
        {
            myIndicatorData.myIndicator.UpdatePosition(StageManager.ourInstance.GetEntityGridPosition(this), nextPosition);
            myIndicatorData.myIndicator.Show();
        }
        else
        {
            myIndicatorData.myIndicator.Hide();
        }
    }

    private bool ComputeCurrentStep(ref Vector2Int aNextPosition)
    {
        aNextPosition = StageManager.ourInstance.GetEntityGridPosition(this) + DirectionToVec(mySteps[myStepsIndex]);

        if (!StageManager.ourInstance.IsPositionInGrid(aNextPosition))
            return false;

        if (!StageManager.ourInstance.CanEntityMoveToPosition(this, aNextPosition))
        {
            Entity entity = StageManager.ourInstance.GetEntity(aNextPosition);

            // Show next indicator if it is the player
            return entity is Player;
        }

        Tile tile = StageManager.ourInstance.GetTile(aNextPosition);
        if (tile is HoleTile holeTile && !holeTile.myIsFilled)
        {
            return false;
        }

        return true;
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

    protected override bool Move(Direction aDirection, System.Func<Tile, bool> aMovementFilterCallback = null)
    {
        if (myIsLerping)
        {
            transform.position = myLerpTarget;
            myLerpT = 0;
        }

        Vector3 position = transform.position;
        myLerpStart = position;

        if (base.Move(aDirection, aMovementFilterCallback))
        {
            myIsLerping = true;
            myLerpTarget = transform.position;
            transform.position = position;
            return true;
        }
                
        return false;
    }
}