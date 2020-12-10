using UnityEngine;
using UnityEngine.Events;

public class SpikesTileAnimator : MonoBehaviour
{
    [Tooltip("A multiplier for all moving animations")]
    [SerializeField] private float myMoveSpeedMultiplier = 1.0f;

    private Animator myAnimator;

    public event UnityAction myOnTurnAnimationEnd;
    public bool myIsInTurnAnimation
    {
        get
        {
            AnimatorStateInfo animatorStateInfo = myAnimator.GetCurrentAnimatorStateInfo(0);

            return myAnimator.IsInTransition(0) || !animatorStateInfo.IsName("Idle");
        }
    }

    public bool myIsInTransition => myAnimator.IsInTransition(0);

    public bool myIsInIdleState => myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle");

    public bool IsInState(string aState)
    {
        return myAnimator.GetCurrentAnimatorStateInfo(0).IsName(aState);
    }

    public bool IsNextState(string aState)
    {
        return myAnimator.IsInTransition(0) && myAnimator.GetNextAnimatorStateInfo(0).IsName(aState);
    }

    private void Awake()
    {
        myAnimator = GetComponentInChildren<Animator>();
        if (myAnimator == null)
        {
            Debug.LogError("SpikesTile does not have animator on model", this);
            enabled = false;
        }
        myAnimator.SetFloat("Speed", myMoveSpeedMultiplier);
    }

    public void Emerge()
    {
        myAnimator.SetBool("Is Burrowed", false);
    }

    public void Burrow()
    {
        myAnimator.SetBool("Is Burrowed", true);
    }

    public void Attack()
    {
        myAnimator.SetTrigger("Attack");
    }

    /// <summary>
    /// Called from Animation Events when a Turn Animation has ended
    /// </summary>
    private void OnTurnAnimationEnded()
    {
        myOnTurnAnimationEnd?.Invoke();
    }
}
