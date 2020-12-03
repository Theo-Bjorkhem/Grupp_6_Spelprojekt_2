using UnityEngine;

public class LockedBox : Entity
{
    private Animator myAnimator;

    public override InteractResult Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);

        if (StageManager.ourInstance.myKeyCount > 0)
        {
            if (AudioManager.ourInstance != null)
            {
                AudioManager.ourInstance.PlaySound("UnlockChest");
            }
            
            StageManager.ourInstance.UnregisterEntity(this);
            myAnimator.SetTrigger("ChestScaleGone");

            --StageManager.ourInstance.myKeyCount;

            return InteractResult.Unlocked;
        }
		return InteractResult.UnlockFailed;
    }

    private void AnimationFinished()
    {
        this.gameObject.SetActive(false);
    }

    void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

}
