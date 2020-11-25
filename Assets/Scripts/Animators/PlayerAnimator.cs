using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [Tooltip("A transform used to rotate the player model")]
    [SerializeField] private Transform myRotationalRoot;

    private Animator myAnimator;

    public event UnityAction myOnTurnAnimationEnd;
    public bool myIsInTurnAnimation { get; private set; }

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myIsInTurnAnimation = false;
    }

    private void FinishTurn()
    {
        myIsInTurnAnimation = false;
        myOnTurnAnimationEnd?.Invoke();
    }

    public void Hop(Direction aDirection)
    {
        myIsInTurnAnimation = true;
        RotateTowardsDirection(aDirection);
        myAnimator.SetTrigger("Hop");
    }

    // Called from animation event
    private void OnHopEnd()
    {
        FinishTurn();
        myAnimator.SetTrigger("Idle");
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
