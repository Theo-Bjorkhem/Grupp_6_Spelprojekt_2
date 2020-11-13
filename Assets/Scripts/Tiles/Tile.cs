using UnityEngine;

public class Tile : MonoBehaviour
{
    /// <summary>
    /// Called when an entity steps on this tile.
    /// By Default this does nothing but can be used to trigger falling, spikes etc.
    /// </summary>
    /// <param name="steppedOnMe"></param>
    public virtual void OnEnter(Entity steppedOnMe)
    {
        return;
    }

    /// <summary>
    /// Called when an entity leaves this tile.
    /// By Default this does nothing, but can be used to break ice etc.
    /// </summary>
    /// <param name="steppedOffMe"></param>
    public virtual void OnExit(Entity steppedOffMe)
    {
        return;
    }

    /// <summary>
    /// A Check called by entities who want to enter this tile which returns true if they are allowed to.
    /// By Default this returns true.
    /// </summary>
    /// <param name="wantsToEnter"></param>
    /// <returns></returns>
    public virtual bool CanEnter(Entity wantsToEnter)
    {
        return true;
    }

    protected virtual void Start()
    {
        StageManager.ourInstance.RegisterTile(this);
    }

}
