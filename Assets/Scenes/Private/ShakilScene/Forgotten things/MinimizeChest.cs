using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimizeChest : Entity
{
    private Animator myAnimator;


    public override InteractResult Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);
        print(StageManager.ourInstance.myUnusedKeys);

        if (StageManager.ourInstance.myUnusedKeys > 0)
        {
            StageManager.ourInstance.myUnusedKeys--;
            if (AudioManager.ourInstance != null)
            {
                AudioManager.ourInstance.PlaySound("UnlockChest");
            }
            
            StageManager.ourInstance.UnregisterEntity(this);
            this.gameObject.SetActive(false);
            return InteractResult.Unlocked;
        }
        return InteractResult.UnlockFailed;
    }

}
