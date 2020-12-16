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
    /// Returns a <see cref="InteractResult"/> to let the interactor know what happened.
    /// </summary>
    /// <param name="aDirection"></param>
    /// <param name="anEntity">The entity interacting with this entity.</param>
    public virtual InteractResult Interact(Entity anEntity, Direction aDirection)
    {
        return InteractResult.Nothing;
    }

    /// <summary>
    /// Remove the entity from the <see cref="StageManager"/> immediately.
    /// Might start any death animations required.
    /// </summary>
    /// <param name="aReason">Reason the entity was killed</param>
    public virtual void Kill(DeathReason aReason)
    {
        StageManager.ourInstance.UnregisterEntity(this);
        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("KillEntity");
        }

        myEntityState |= EntityState.Dead;
    }

    /// <summary>
    /// Ask stageManager what is in the adjacent space in the arguments direction.
    /// Move there if possible. Return true on successful move, otherwise false.
    /// </summary>
    /// <param name="aDirection"></param>
    protected virtual bool Move(Direction aDirection, System.Func<Tile, bool> aMovementFilterCallback = null)
    {
        Vector2Int gridPosition = StageManager.ourInstance.GetEntityGridPosition(this);
        gridPosition += DirectionToVec(aDirection);

        Tile nextTile = StageManager.ourInstance.IsPositionInGrid(gridPosition) ? StageManager.ourInstance.GetTile(gridPosition) : null;
        if (StageManager.ourInstance.CanEntityMoveToPosition(this, gridPosition) && (aMovementFilterCallback == null || aMovementFilterCallback(nextTile)))
        {
            StageManager.ourInstance.MoveEntity(this, gridPosition);
            if (AudioManager.ourInstance != null)
            {
                AudioManager.ourInstance.PlaySound(myMoveSound);
            }
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

    protected Direction VecToDirection(Vector2Int aVector)
    {
        if (aVector == Vector2Int.up)
        {
            return Direction.Up;
        }
        else if (aVector == Vector2Int.right)
        {
            return Direction.Right;
        }
        else if (aVector == Vector2Int.down)
        {
            return Direction.Down;
        }
        else if (aVector == Vector2Int.left)
        {
            return Direction.Left;
        }
        else
        {
            Debug.Assert(false, "Invalid direction vector", this);
            return Direction.Up;
        }
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

#if UNITY_EDITOR

    private StageManager myStageManagerUE;
    private void OnDrawGizmosSelected()
    {
        if (myStageManagerUE == null)
        {
            myStageManagerUE = FindObjectOfType<StageManager>();

            if (myStageManagerUE == null)
            {
                return;
            }
        }

        if (!enabled)
        {
            return;
        }

        Vector2Int gridPosition = myStageManagerUE.GetTilePositionFromWorldTile(transform.position);

        Vector3 center = myStageManagerUE.GetEntityWorldPositionFromTilePosition(gridPosition) - new Vector3(myStageManagerUE.myTileSize, 0.0f, myStageManagerUE.myTileSize) * 0.5f;

        Color color = Color.green;

        if (!myStageManagerUE.IsPositionInGrid(gridPosition))
        {
            color = Color.red;
        }
        else if (!Mathf.Approximately(transform.position.x % 1.0f, 0.0f) || !Mathf.Approximately(transform.position.z % 1.0f, 0.0f))
        {
            color = Color.yellow;
        }

        color.a = 0.5f;

        Gizmos.color = color;
        Gizmos.DrawCube(center, new Vector3(myStageManagerUE.myTileSize, 0.1f, myStageManagerUE.myTileSize));
    }

#endif
}