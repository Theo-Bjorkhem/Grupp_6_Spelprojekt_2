using UnityEngine;

public partial class Player : Entity
{
    public bool myIsInTurn => myTurnEvent != null;
    public bool myIsAcceptingInput => !myAnimator.myIsInTurnAnimation && myWasLastActionFullyHandled;

    [SerializeField]
    private TouchConfiguration myTouchConfiguration = TouchConfiguration.Default;

    private TurnEvent myTurnEvent = null;

    private TouchProgress myTouchProgress = new TouchProgress();

    private bool myWasLastActionFullyHandled = true;

    private Camera myMainCamera;
    private PlayerAnimator myAnimator;

    public PlayerAnimator GetAnimator()
    {
        return myAnimator;
    }

    public override void Action(TurnEvent aTurnEvent)
    {
        myTurnEvent = aTurnEvent;
    }

    public override void Kill(DeathReason aReason)
    {
        base.Kill(aReason);

        myAnimator.StopVFX();
    }

    protected override void Start()
    {
        base.Start();

        myMainCamera = Camera.main;
        myAnimator = GetComponentInChildren<PlayerAnimator>();
        if (myAnimator == null)
        {
            enabled = false;
            Debug.Assert(false, "No Animator-component found in children of PlayerAnimator!", this);
        }

        myTouchProgress.myTouchConfiguration = myTouchConfiguration;
    }

    private void Update()
    {
        if (IsDead())
        {
            return;
        }

        if (myIsInTurn)
        {
            PlayerAction();
        }
    }

    private System.Collections.IEnumerator DoPush(Direction aMovementDirection, MoveableBox aMoveableBox)
    {
        // NOTE: Special cased the HoleTile for now,
        // if we pushed a box that was on a BreakableTile and it broke and is now a HoleTile
        // we should not be able to move into that tile and die immediately.
        bool couldMove = Move(aMovementDirection, tile => !(tile is HoleTile));

        if (IsDead())
        {
            SetTurnHandled();
            yield break;
        }

        PushAction(aMovementDirection, !couldMove);

        // Wait a frame for animator to start transition
        yield return null;

        while (myAnimator.myIsInTransition || !myAnimator.myIsInIdleState)
            yield return null;

        if (!myWasLastActionFullyHandled)
        {
            SetTurnHandled();
        }
    }

    private void SetTurnHandled()
    {
        Debug.Assert(myTurnEvent != null, "Cannot set turn handled when not in turn!");

        myWasLastActionFullyHandled = true;

        myTouchProgress.Reset();

        myTurnEvent.SignalDone();
        myTurnEvent = null;
    }

    private void PlayerAction()
    {
        if (!myIsAcceptingInput)
        {
            return;
        }
        
        TurnActionData turnActionData = GetTurnActionFromInput();

        if (turnActionData.myType == TurnActionData.Type.Move)
        {
            myWasLastActionFullyHandled = HandleNormalMovement(turnActionData.myMoveDirection);
        }

        if (turnActionData.myConsumesTurn && myWasLastActionFullyHandled)
        {
            SetTurnHandled();
        }
    }

    private TurnActionData GetTurnActionFromInput()
    {
        myTouchProgress.Update();

        if (myTouchProgress.myHasCompleteEvent)
        {
            TouchEvent touchEvent = myTouchProgress.myTouchEvent;

            if (touchEvent.myType == TouchEvent.Type.Swipe)
            {
                return TurnActionData.CreateMove(touchEvent.mySwipeDirection);
            }
        }

#if UNITY_EDITOR

        //Keyboard Input (for convenience)
        Direction moveDirection = Direction.Up;
        bool gotInput = false;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            gotInput = true;
            moveDirection = Direction.Up;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            gotInput = true;
            moveDirection = Direction.Right;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            gotInput = true;
            moveDirection = Direction.Left;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            gotInput = true;
            moveDirection = Direction.Down;
        }

        if (gotInput)
        {
            return TurnActionData.CreateMove(moveDirection);
        }

#endif

        return TurnActionData.None;
    }

    private Entity FindEntityFromScreenClick(Vector3 aScreenPos)
    {
        Ray ray = myMainCamera.ScreenPointToRay(aScreenPos);

        int layerMask = LayerMask.GetMask("Selectable Entity");

        if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, layerMask, QueryTriggerInteraction.Collide))
        {
            return hitInfo.collider.gameObject.GetComponent<Entity>();
        }

        return null;
    }

    /// <returns>true if the action was fully handled this frame, false otherwise.</returns>
    private bool HandleNormalMovement(Direction aMovementDirection)
    {
        Entity entityAtNextPosition = GetEntityInDirection(aMovementDirection);
        InteractResult interactResult = InteractResult.Undefined;

        if (entityAtNextPosition != null)
        {
            if (entityAtNextPosition.GetType() == typeof(EnemyPath))
            {
                Kill(DeathReason.Enemy);
            }

            interactResult = entityAtNextPosition.Interact(this, aMovementDirection);
            
            switch (interactResult)
            {
                case InteractResult.BoxMoved:
                    StartCoroutine(DoPush(aMovementDirection, entityAtNextPosition as MoveableBox));
                    return false;
                case InteractResult.BoxMoveFailed:
                    KickAction(aMovementDirection);
                    break;

                case InteractResult.KeyPickedUp:
                case InteractResult.Unlocked:
                    Move(aMovementDirection);
                    MoveAction(aMovementDirection);
                    break;

                default:
                    BlockedAction(aMovementDirection);
                    break;
            }
        }
        else if (Move(aMovementDirection))
        {
            MoveAction(aMovementDirection);
        }
        else
        {
            BlockedAction(aMovementDirection);
        }

        return true;
    }

    private Entity GetEntityInDirection(Direction aDirection)
    {
        Vector2Int checkPosition = StageManager.ourInstance.GetEntityGridPosition(this) + DirectionToVec(aDirection);

        if (StageManager.ourInstance.IsPositionInGrid(checkPosition))
        {
            return StageManager.ourInstance.GetEntity(checkPosition);
        }
        else
        {
            return null;
        }
    }

    private void MoveAction(Direction aMovementDirection)
    {
        myAnimator.Move(aMovementDirection);

        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("PlayerMove");
        }
    }

    private void BlockedAction(Direction aMovementDirection)
    {
        myAnimator.Blocked(aMovementDirection);

        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("MoveIntoWall");
        }
    }

    private void KickAction(Direction aMovementDirection)
    {
        myAnimator.Kick(aMovementDirection);

        // kick sound?
        //if (AudioManager.ourInstance != null)
        //{
        //    AudioManager.ourInstance.PlaySound("");
        //}
    }

    private void PushAction(Direction aMovementDirection, bool aBoxDropped)
    {
        if (aBoxDropped)
        {
            KickAction(aMovementDirection);
        }
        else
        {
            myAnimator.Push(aMovementDirection);
        }
    }
}
