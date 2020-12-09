using UnityEngine;

public class MoveableBox : Entity
{
    public bool myIsInHole => myHoleTile != null;

    private HoleTile myHoleTile;

    private Collider myCollider;

    private bool myIsInMove = false;

    private MoveableBoxAnimator myAnimator;

    public override InteractResult Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);

        if (myAnimator.myPlayerAnimator == null && anEntity is Player player)
        {
            myAnimator.myPlayerAnimator = player.GetAnimator();
        }

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

        if (!myIsInMove)
        {
            TriggerFallAnimation();
        }
    }

    protected override bool Move(Direction aDirection, System.Func<Tile, bool> aMovementFilterCallback = null)
    {
        myIsInMove = true;

        myAnimator.PreparePossibleMove();

        Vector3 preMovePos = transform.position;

        bool result = base.Move(aDirection, aMovementFilterCallback);

        if (result)
        {
            myAnimator.DoMove(preMovePos, transform.position);
        }

        if (myIsInHole)
        {
            TriggerFallAnimation();
        }

        myIsInMove = false;

        return result;
    }

    private void TriggerFallAnimation()
    {
        myAnimator.DoFall();
    }

    private void Awake()
    {
        myAnimator = GetComponent<MoveableBoxAnimator>();

        myCollider = GetComponent<Collider>();
        myMoveSound = "BoxMove";
    }
}
