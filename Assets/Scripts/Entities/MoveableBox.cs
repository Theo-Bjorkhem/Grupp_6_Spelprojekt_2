using UnityEngine;

public class MoveableBox : Entity
{
    public bool myIsInHole => myHoleTile != null;

    private HoleTile myHoleTile;

    public override void Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);       
        Move(aDirection);

        if (myIsInHole)
        {
            // TODO: Fall animation
            // TODO: * 0.5f until pivot is correct on entities
            transform.position += Vector3.down * StageManager.ourInstance.myTileSize * 0.5f;
        }
    }

    public void OnFellInHole(HoleTile aHoleTile)
    {
        myHoleTile = aHoleTile;

        StageManager.ourInstance.UnregisterEntity(this);
    }

}
