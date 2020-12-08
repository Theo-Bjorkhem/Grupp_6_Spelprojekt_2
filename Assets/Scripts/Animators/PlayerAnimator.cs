using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [Tooltip("A transform used to rotate the player model")]
    [SerializeField] private Transform myRotationalRoot;

    [Tooltip("A multiplier for all moving animations")]
    [SerializeField] private float myMoveSpeedMultiplier = 1.0f;

    [Tooltip("A transform that is used during animations to animate position shifting")]
    [SerializeField] private Transform myAnimationPositionRoot;

    private Animator myAnimator;

    public event UnityAction myOnTurnAnimationEnd;
    public bool myIsInTurnAnimation
    {
        get
        {
            AnimatorStateInfo animatorStateInfo = myAnimator.GetCurrentAnimatorStateInfo(0);

            return myAnimator.IsInTransition(0) || !(animatorStateInfo.IsName("Idle") || animatorStateInfo.IsName("Grabbing"));
        }
    }

    public bool myIsInTransition => myAnimator.IsInTransition(0);

    public bool myIsInGrabbingState => myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Grabbing");

    public bool myIsInIdleState => myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle");

    public bool IsInState(string aState)
    {
        return myAnimator.GetCurrentAnimatorStateInfo(0).IsName(aState);
    }

    public bool IsNextState(string aState)
    {
        return myAnimator.IsInTransition(0) && myAnimator.GetNextAnimatorStateInfo(0).IsName(aState);
    }

    public Transform GetAnimationPositionRoot()
    {
        return myAnimationPositionRoot;
    }

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myAnimator.SetFloat("Movement Animation Speed Multiplier", myMoveSpeedMultiplier);
    }

    public void Fall()
    {
        RotateTowardsDirection(Direction.Down);
        myAnimator.SetTrigger("Fall");
    }

    public void Death()
    {
        RotateTowardsDirection(Direction.Down);
        myAnimator.SetTrigger("Death");
    }

    public void LetGo()
    {
        myAnimator.SetBool("IsGrabbing", false);
    }

    public void Grab(Direction aDirection)
    {
        RotateTowardsDirection(aDirection);
        myAnimator.SetBool("IsGrabbing", true);
    }

    public void Pull(Direction aDirection)
    {
        RotateTowardsDirection(aDirection);
        myAnimator.SetTrigger("Pull");
    }

    public void Push(Direction aDirection)
    {
        RotateTowardsDirection(aDirection);
        myAnimator.SetTrigger("Push");
    }

    public void Blocked(Direction aDirection)
    {
        RotateTowardsDirection(aDirection);
        myAnimator.SetTrigger("Blocked");
    }

    public void Move(Direction aDirection)
    {
        RotateTowardsDirection(aDirection);

        myAnimator.SetTrigger("Move");
    }

    public void Kick(Direction aDirection)
    {
        RotateTowardsDirection(aDirection);

        myAnimator.SetTrigger("Kick");
    }

    /// <summary>
    /// Called from Animation Events when a Turn Animation has ended
    /// </summary>
    private void OnTurnAnimationEnded()
    {
        myOnTurnAnimationEnd?.Invoke();
    }

    private void RotateTowardsDirection(Direction aDirection)
    {
        switch (aDirection)
        {
            case Direction.Up:
                myRotationalRoot.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
            case Direction.Right:
                myRotationalRoot.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                break;
            case Direction.Down:
                myRotationalRoot.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                break;
            case Direction.Left:
                myRotationalRoot.localRotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
                break;
            default:
                break;
        }
    }
}
