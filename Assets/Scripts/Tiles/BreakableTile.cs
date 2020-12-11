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

    private Animator myAnimator;

    public override void OnEnter(Entity steppedOnMe)
    {
        base.OnEnter(steppedOnMe);

        if (CheckCanEntityTriggersStep(steppedOnMe))
        {
            HandleStep();
        }
    }

    public override void OnExit(Entity steppedOffMe)
    {
        base.OnExit(steppedOffMe);

        if (CheckCanEntityTriggersStep(steppedOffMe))
        {
            HandleStep();
        }
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
            if (AudioManager.ourInstance != null)
            {
                AudioManager.ourInstance.PlaySound("BreakableTileBreaks");
            }

            // TODO: Animation etc...
            myAnimator.SetTrigger("TileBreaks");

            Entity entity = StageManager.ourInstance.GetEntity(StageManager.ourInstance.GetTilePositionFromWorldTile(transform.position));

            if (entity != null && myHoleTilePrefab == null)
            {
                entity.Kill(DeathReason.Fall);
            }

            StageManager.ourInstance.UnregisterTile(this);

            if (myHoleTilePrefab != null)
            {
                // If we have a hole tile prefab we'll spawn it as a replacement for ourselves.
                Instantiate(myHoleTilePrefab, transform.position, Quaternion.identity);
            }
        }

        return false;
    }

    private bool CheckCanEntityTriggersStep(Entity anEntity)
    {
        return
            anEntity is Player ||
            anEntity is EnemyPath ||
            anEntity is EnemySeek ||
            anEntity is MoveableBox;
    }

    /// <summary>
    /// Called from animation event
    /// </summary>
    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }
}
