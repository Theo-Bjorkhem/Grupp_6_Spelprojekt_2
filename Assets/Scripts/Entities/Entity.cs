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
    public virtual void Interact(Direction aDirection)
    {

    }

    /// <summary>
    /// Ask stageManager what is in the adjacent space in the arguments direction.
    /// Move there is possible, otherwise interact with it.
    /// </summary>
    /// <param name="aDirection"></param>
    protected virtual void Move(Direction aDirection)
    {

    }

    protected virtual void Start()
    {
        StageManager.ourInstance.RegisterEntity(this);
    }
}