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
    public void Action(TurnEvent aTurnEvent)
    {
        aTurnEvent.SignalDone();
    }

    /// <summary>
    /// Ask stageManager what is in the adjacent space in the arguments direction.
    /// Move there is possible, otherwise interact with it.
    /// </summary>
    /// <param name="aDirection"></param>
    private void Move(Direction aDirection)
    {

    }

    /// <summary>
    /// When "moved" into, by the player usually. This is where a box is pushed for example.
    /// </summary>
    /// <param name="aDirection"></param>
    public void Interact(Direction aDirection)
    {

    }

    private void Start()
    {
        StageManager.ourInstance.RegisterEntity(this);
    }
}