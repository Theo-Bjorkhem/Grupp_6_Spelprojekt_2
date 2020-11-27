using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Entity
{
    public override void Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);

        StageManager.ourInstance.myHasKey = true;

        StageManager.ourInstance.UnregisterEntity(this);
        this.gameObject.SetActive(false);
    }
}
