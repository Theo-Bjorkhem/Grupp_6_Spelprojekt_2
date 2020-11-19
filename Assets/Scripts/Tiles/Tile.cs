using UnityEngine;

public class Tile : MonoBehaviour
{
    [Tooltip("If true this tile will not allow any entity to step on it.")]
    [SerializeField]
    protected bool myForceDisallowEnter = false;

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
    /// By Default this returns !<see cref="myForceDisallowEnter"/>.
    /// </summary>
    /// <param name="wantsToEnter"></param>
    /// <returns></returns>
    public virtual bool CanEnter(Entity wantsToEnter)
    {
        return !myForceDisallowEnter;
    }

    /// <summary>
    /// Called from <see cref="StageManager"/> for tiles that have subscribed for turn events through <see cref="StageManager.RegisterTileForTurnEvents(Tile)"/>.
    /// </summary>
    /// <remarks>
    /// Tile turn events run after all entities in current turn.
    /// </remarks>
    /// <param name="aTurnEvent"></param>
    public virtual void OnTurnEvent(TurnEvent aTurnEvent)
    {
        aTurnEvent.SignalDone();
    }

    protected virtual void Start()
    {
        StageManager.ourInstance.RegisterTile(this);
    }

}
