using UnityEngine;

public class BreakableTile : Tile
{
    [Tooltip("Set to prefab of the HoleTile to spawn when tile breaks. (if null, no HoleTile will be spawned)")]
    [SerializeField]
    private GameObject myHoleTilePrefab;

    [Tooltip("Amounts of times entities can step on this tile before it breaks.")]
    [SerializeField]
    [Min(0)]
    private int myBreakThreshold = 2;

    private int myStepCount = 0;

    public override void OnEnter(Entity steppedOnMe)
    {
        base.OnEnter(steppedOnMe);

        HandleStep();
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

            if (myHoleTilePrefab != null)
            {
                // If we have a hole tile prefab we'll spawn it as a replacement for ourselves.
                Instantiate(myHoleTilePrefab, transform.position, Quaternion.identity);
            }

            return true;
        }

        return false;
    }
}
