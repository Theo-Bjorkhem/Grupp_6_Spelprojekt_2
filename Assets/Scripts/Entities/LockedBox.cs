﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedBox : Entity
{
    private Animator myAnimator;

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
            myAnimator.SetTrigger("ChestScaleGone");

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
