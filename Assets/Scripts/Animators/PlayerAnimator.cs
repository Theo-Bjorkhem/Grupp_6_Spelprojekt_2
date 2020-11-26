using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [Tooltip("A transform used to rotate the player model")]
    [SerializeField] private Transform myRotationalRoot;
    [Tooltip("A multiplier for all moving animations")]
    [SerializeField] private float myMoveSpeedMultiplier = 1.0f;

    private Animator myAnimator;

    public event UnityAction myOnTurnAnimationEnd;
    public bool myIsInTurnAnimation { get; private set; }

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myAnimator.SetFloat("Movement Animation Speed Multiplier", myMoveSpeedMultiplier);
        myIsInTurnAnimation = false;
    }

    public void Blocked(Direction aDirection)
    {
        myIsInTurnAnimation = true;
        RotateTowardsDirection(aDirection);
        myAnimator.SetTrigger("Blocked");
    }

    public void Move(Direction aDirection)
    {
        myIsInTurnAnimation = true;
        RotateTowardsDirection(aDirection);
        myAnimator.SetTrigger("Move");
    }

    /// <summary>
    /// Called from Animation Events when a Turn Animation has ended
    /// </summary>
    private void OnTurnAnimationEnded()
    {
        myIsInTurnAnimation = false;
        myAnimator.SetTrigger("Idle");
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
