using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KeyAnimator))]
public class OpenChest : Entity
{
    //Animator animator;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    animator = GetComponent<Animator>();
    //}

    //private void OnTriggerEnter(Collider collider)
    //{
    //    if(collider.tag == "Player")
    //    {
    //        animator.SetTrigger("OpenChest");
    //    }
    //}

    // animator.SetTrigger("OpenChest");

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
            this.gameObject.SetActive(false);
            return InteractResult.Unlocked;
        }
        return InteractResult.UnlockFailed;
    }
}

