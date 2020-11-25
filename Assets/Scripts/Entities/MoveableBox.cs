using UnityEngine;

public class MoveableBox : Entity
{
    public bool myIsInHole => myHoleTile != null;
    public bool myIsGrabbedByPlayer => myGrabbedByPlayer != null;

    private HoleTile myHoleTile;

    private Player myGrabbedByPlayer;

    private Collider myCollider;

    public override void Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);       
        
        Move(aDirection);
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
    }

    protected override bool Move(Direction aDirection)
    {
        bool didMove = base.Move(aDirection);

        if (myIsInHole)
        {
            // TODO: Fall animation
            // TODO: * 0.5f until pivot is correct on entities
            transform.position += Vector3.down * StageManager.ourInstance.myTileSize * 0.5f;
        }

        return didMove;
    }

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }
}
