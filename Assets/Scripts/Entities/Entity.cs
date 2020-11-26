using UnityEngine;

public class Entity : MonoBehaviour
{
    private EntityState myEntityState = EntityState.Normal;
    [HideInInspector]
    public string myMoveSound = "PlayerMove";

    public virtual bool IsDead() => myEntityState.HasFlag(EntityState.Dead);

    /// <summary>
    /// Default: The entity does nothing.
    /// Otherwise: Moving entities attempt to Move(). Spikes toggle. Player waits for input.
    /// </summary>
    /// <remarks>
    /// Ensure to call <see cref="TurnEvent.SignalDone"/> on <paramref name="aTurnEvent"/> at some point to signal this entity has finished its turn!
    /// </remarks>
    /// <param name="aTurnEvent"></param>
    public virtual void Action(TurnEvent aTurnEvent)
    {
        aTurnEvent.SignalDone();
    }

    /// <summary>
    /// When "moved" into, by the player usually. This is where a box is pushed for example.
    /// </summary>
    /// <param name="aDirection"></param>
    /// <param name="anEntity">The entity interacting with this entity.</param>
    public virtual void Interact(Entity anEntity, Direction aDirection)
    {
        
    }

    /// <summary>
    /// Remove the entity from the <see cref="StageManager"/> immediately.
    /// Might start any death animations required.
    /// </summary>
    /// <param name="aReason">Reason the entity was killed</param>
    public virtual void Kill(DeathReason aReason)
    {
        StageManager.ourInstance.UnregisterEntity(this);

        myEntityState |= EntityState.Dead;
    }

    /// <summary>
    /// Ask stageManager what is in the adjacent space in the arguments direction.
    /// Move there if possible. Return true on successful move, otherwise false.
    /// </summary>
    /// <param name="aDirection"></param>
    protected virtual bool Move(Direction aDirection)
    {
        Vector2Int gridPosition = StageManager.ourInstance.GetEntityGridPosition(this);
        gridPosition += DirectionToVec(aDirection);

        if (StageManager.ourInstance.CanEntityMoveToPosition(this, gridPosition))
        {
            StageManager.ourInstance.MoveEntity(this, gridPosition);
            AudioManager.ourInstance?.PlaySound(myMoveSound);
            transform.position = StageManager.ourInstance.GetEntityWorldPositionFromTilePosition(gridPosition);

            return true;
        }

        return false;
    }

    protected virtual void Start()
    {
        StageManager.ourInstance.RegisterEntity(this);
    }

    protected Vector2Int DirectionToVec(Direction aDirection)
    {
        switch (aDirection)
        {
            case Direction.Up:
                return Vector2Int.up;
            case Direction.Right:
                return Vector2Int.right;
            case Direction.Down:
                return Vector2Int.down;
            case Direction.Left:
                return Vector2Int.left;
            default:
                Debug.LogError(this + " has no direction somehow.");
                break;
        }
        return Vector2Int.up;
    }

    protected Direction ReverseDirection (Direction aDirection)
    {
        switch (aDirection)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Right:
                return Direction.Left;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            default:
                Debug.LogError(this + " has no direction somehow.");
                break;
        }
        return Direction.Up;
    }
}