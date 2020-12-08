using UnityEngine;

public class MoveableBox : Entity
{
    public bool myIsInHole => myHoleTile != null;

    private HoleTile myHoleTile;

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

    public void OnFellInHole(HoleTile aHoleTile)
    {
        myHoleTile = aHoleTile;

        myCollider.enabled = false;

        StageManager.ourInstance.UnregisterEntity(this);

        TriggerFallAnimation();
    }

    protected override bool Move(Direction aDirection, System.Func<Tile, bool> aMovementFilterCallback = null)
    {
        bool result = base.Move(aDirection, aMovementFilterCallback);

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
