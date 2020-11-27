using UnityEngine;

public class BreakableTile : Tile
{
    [Tooltip("Amounts of times entities can step on this tile before it breaks.")]
    [SerializeField]
    [Min(0)]
    private int myBreakThreshold = 2;

    private int myStepCount = 0;

    public override void OnEnter(Entity steppedOnMe)
    {
        base.OnEnter(steppedOnMe);

        // TODO: write a better solution for entities that do not break tiles
        if (steppedOnMe as Key == null)
        {
            HandleStep();
        }
    }

    public override void OnExit(Entity steppedOffMe)
    {
        base.OnExit(steppedOffMe);

        HandleStep();
    }

    public override bool CanEnter(Entity wantsToEnter)
    {
        return myStepCount < myBreakThreshold;
    }

    private bool HandleStep()
    {
        ++myStepCount;

        if (myStepCount >= myBreakThreshold)
        {
            // TODO: Animation etc...
            gameObject.SetActive(false);

            Entity entity = StageManager.ourInstance.GetEntity(StageManager.ourInstance.GetTilePositionFromWorldTile(transform.position));

            if (entity != null)
            {
                entity.Kill(DeathReason.Fall);
            }

            // This requires that no entities are present on the tile, killing an entity should remove it from the grid immediately.
            StageManager.ourInstance.UnregisterTile(this);

            return true;
        }

        return false;
    }
}
