using UnityEngine;

public class Entity : MonoBehaviour
{
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
    /// Ask stageManager what is in the adjacent space in the arguments direction.
    /// Move there is possible, otherwise interact with it.
    /// </summary>
    /// <param name="aDirection"></param>
    protected virtual void Move(Direction aDirection)
    {
        Vector2Int position = StageManager.ourInstance.GetTilePositionFromWorld(transform.position);

        switch (aDirection)
        {
            case Direction.Up:
                position += Vector2Int.up;
                break;
            case Direction.Right:
                position += Vector2Int.right;
                break;
            case Direction.Down:
                position += Vector2Int.down;
                break;
            case Direction.Left:
                position += Vector2Int.left;
                break;
            default:
                Debug.LogError(this + " has no direction, somehow.");
                break;
        }

        if (StageManager.ourInstance.CanEntityMoveToPosition(this, position))
        {
            transform.position = new Vector3(position.x, 0, position.y);

            StageManager.ourInstance.MoveEntity(this, position);
        }
    }

    protected virtual void Start()
    {
        StageManager.ourInstance.RegisterEntity(this);
    }
}