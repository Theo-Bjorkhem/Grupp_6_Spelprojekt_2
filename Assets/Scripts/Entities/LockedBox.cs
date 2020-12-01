using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedBox : Entity
{
    public override InteractResult Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);
        print(StageManager.ourInstance.myHasKey);

        if (StageManager.ourInstance.myHasKey)
        {
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
