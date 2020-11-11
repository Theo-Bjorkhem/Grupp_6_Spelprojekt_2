using UnityEngine;

public class Entity : MonoBehaviour
{
    /// <summary>
    /// Default: Return. The entity does nothing.
    /// Otherwise: Moving entities attempt to Move(). Spikes toggle. Player waits for input.
    /// </summary>
    public void Action()
    {
        return;
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
}