using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedBox : Entity
{

    public override InteractResult Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);

        if (StageManager.ourInstance.myHasKey)
        {
            StageManager.ourInstance.UnregisterEntity(this);
            this.gameObject.SetActive(false);
            return InteractResult.Unlocked;
        }

        return InteractResult.UnlockFailed;
    }

}
