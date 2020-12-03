using UnityEngine;

public class MoveableBox : Entity
{
    public bool myIsInHole => myHoleTile != null;
    public bool myIsGrabbedByPlayer => myGrabbedByPlayer != null;

    private HoleTile myHoleTile;

    private Player myGrabbedByPlayer;

    private Collider myCollider;

    public override InteractResult Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);       
        
        if (Move(aDirection))
        {
            return InteractResult.BoxMoved;
        }
        else
        {
            return InteractResult.BoxMoveFailed;
        }
    }

    public bool OnPlayerMoveBox(Direction aDirection)
    {
        return Move(aDirection);
    }

    public void OnGrabbedByPlayer(Player aPlayer)
    {
        Debug.Assert(!myIsGrabbedByPlayer, "OnGrabbedByPlayer called when already grabbed!");

        myGrabbedByPlayer = aPlayer;
    }

    public void OnReleasedByPlayer()
    {
        Debug.Assert(myIsGrabbedByPlayer, "OnReleasedByPlayer called when not grabbed!");

        myGrabbedByPlayer = null;
    }

    public void OnFellInHole(HoleTile aHoleTile)
    {
        if (myIsGrabbedByPlayer)
        {
            myGrabbedByPlayer.ForceReleaseBox();
        }

        myHoleTile = aHoleTile;

        myCollider.enabled = false;

        StageManager.ourInstance.UnregisterEntity(this);

        TriggerFallAnimation();
    }

    protected override bool Move(Direction aDirection)
    {
        bool result = base.Move(aDirection);

        if (myIsInHole)
        {
            TriggerFallAnimation();
        }

        return result;
    }

    private void TriggerFallAnimation()
    {
        transform.position += Vector3.down * StageManager.ourInstance.myTileSize * 0.9f;
    }

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        myMoveSound = "BoxMove";
    }
}
