using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator myAnimator;

    public Action OnTurnAnimationFinish { private get; set; }

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        OnTurnAnimationFinish = null;
    }

    private void FinishTurn()
    {
        Debug.Assert(OnTurnAnimationFinish != null, "Could not invoke OnTurnAnimationFinish because it was not set", this);
        OnTurnAnimationFinish.Invoke();
    }

    public void Hop()
    {
        myAnimator.SetTrigger("Hop");
    }

    // Called from animation event
    private void OnHopEnd()
    {
        FinishTurn();
        myAnimator.SetTrigger("Idle");
    }
}
