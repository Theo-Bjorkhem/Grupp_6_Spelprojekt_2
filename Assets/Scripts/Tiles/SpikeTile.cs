using UnityEngine;

[RequireComponent(typeof(SpikesTileAnimator))]
public class SpikeTile : Tile
{
    [Tooltip("Should the spikes be extended initially?")]
    [SerializeField]
    private bool myInitialExtended = false;

    private SpikesTileAnimator myAnimator;

    private bool myIsExtended = true;

    public override void OnTurnEvent(TurnEvent aTurnEvent)
    {
        SetExtended(!myIsExtended);

        base.OnTurnEvent(aTurnEvent);
    }

    protected override void Start()
    {
        base.Start();

        myAnimator = GetComponent<SpikesTileAnimator>();
        StageManager.ourInstance.RegisterTileForTurnEvents(this);

        SetExtended(myInitialExtended);
    }

    private void SetExtended(bool anExtend)
    {
        if (anExtend)
        {
            ExtendSpikes();
        }
        else
        {
            RetractSpikes();
        }
    }

    private void ExtendSpikes()
    {
        if (myIsExtended)
            return;

        myIsExtended = true;

        // transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z); // Test visuals
        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("ArmsExtend");
        }

        // If the player is standing on this tile we'll kill it
        Entity entity = StageManager.ourInstance.GetEntity(StageManager.ourInstance.GetTilePositionFromWorldTile(transform.position));
        
        if (entity != null && entity is Player)
        {
            myAnimator.Attack();
            entity.Kill(DeathReason.Spike);
        }
        else
        {
            myAnimator.Emerge();
        }
    }

    private void RetractSpikes()
    {
        if (!myIsExtended)
            return;

        myIsExtended = false;

        // TODO: Rectract spikes visually when spike model has spikes and main body separate
        //transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z); // Test visuals
        myAnimator.Burrow();
        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("ArmsRetract");
        }
    }
}
