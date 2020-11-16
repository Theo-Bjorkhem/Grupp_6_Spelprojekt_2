using UnityEngine;

public class EnemyPatrol : Entity
{
    [SerializeField]
    private Direction myDirection;

    public override void Action(TurnEvent aTurnEvent)
    {
        Vector2Int currentPosition = StageManager.ourInstance.GetTilePositionFromWorld(transform.position);
        Vector2Int newPosition = currentPosition;
        switch (myDirection)
        {
            case Direction.Up:
                newPosition += Vector2Int.up;
                break;
            case Direction.Right:
                newPosition += Vector2Int.right;
                break;
            case Direction.Down:
                newPosition += Vector2Int.down;
                break;
            case Direction.Left:
                newPosition += Vector2Int.left;
                break;
            default:
                Debug.LogError(this + "lacks a direction.");
                break;
        }

        if (StageManager.ourInstance.CanEntityMoveToPosition(this, newPosition))
        {
            Move(myDirection);
            aTurnEvent.SignalDone();
            return;
        }

        switch (myDirection)
        {
            case Direction.Up:
                myDirection = Direction.Down;
                break;
            case Direction.Right:
                myDirection = Direction.Left;
                break;
            case Direction.Down:
                myDirection = Direction.Up;
                break;
            case Direction.Left:
                myDirection = Direction.Right;
                break;
            default:
                Debug.LogError(this + "lacks a direction.");
                break;
        }

        Action(aTurnEvent);
    }

    public override void Interact(Entity anEntity, Direction aDirection)
    {
        if (anEntity is Player)
        {
            //kill player
        }
    }
}