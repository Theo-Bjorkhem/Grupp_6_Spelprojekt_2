using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerAnimator))]
public partial class Player : Entity
{
    public bool myIsInTurn => myTurnEvent != null;
    public bool myIsAcceptingInput => !myAnimator.myIsInTurnAnimation;
    public bool myIsGrabbingBox => myGrabbedBox != null;

    [SerializeField]
    private TouchConfiguration myTouchConfiguration = TouchConfiguration.Default;

    private TurnEvent myTurnEvent = null;

    private TouchProgress myTouchProgress = new TouchProgress();

    // Grabbable box
    private MoveableBox myGrabbedBox;

    private Camera myMainCamera;
    private PlayerAnimator myAnimator;

    public override void Action(TurnEvent aTurnEvent)
    {
        myTurnEvent = aTurnEvent;
    }

    /// <summary>
    /// Called from ex. <see cref="MoveableBox"/> when the player is forced to release the box.
    /// </summary>
    public void ForceReleaseBox()
    {
        OnReleaseBox();
    }

    protected override void Start()
    {
        base.Start();

        myMainCamera = Camera.main;
        myAnimator = GetComponent<PlayerAnimator>();

        myTouchProgress.myTouchConfiguration = myTouchConfiguration;
    }

    private void Update()
    {
        if (myIsInTurn)
        {
            PlayerAction();
        }
    }

    private void OnGrabBox(MoveableBox aBox)
    {
        Debug.Assert(!myIsGrabbingBox, "OnGrabBox called when already grabbing box!");

        myGrabbedBox = aBox;
        myGrabbedBox.OnGrabbedByPlayer(this);

        GrabAction(VecToDirection(
            StageManager.ourInstance.SubtractEntityGridPositions(
                aBox,
                this
        )));
    }

    private void OnReleaseBox()
    {
        Debug.Assert(myIsGrabbingBox, "OnReleaseBox called when not grabbing box!");

        LetGoAction();

        myGrabbedBox.OnReleasedByPlayer();
        myGrabbedBox = null;
    }

    private void PlayerAction()
    {
        if (!myIsAcceptingInput)
        {
            return;
        }
        
        TurnActionData turnActionData = GetTurnActionFromInput();

        switch (turnActionData.myType)
        {
            case TurnActionData.Type.Move:
                if (myIsGrabbingBox)
                {
                    HandleGrabbedMovement(turnActionData.myMoveDirection);
                }
                else
                {
                    HandleNormalMovement(turnActionData.myMoveDirection);
                }
                break;

            case TurnActionData.Type.Box:
                if (myGrabbedBox == turnActionData.myMoveableBox)
                {
                    OnReleaseBox();
                }
                else
                {
                    if (myIsGrabbingBox)
                    {
                        OnReleaseBox();
                    }

                    if (CheckCanGrabBox(turnActionData.myMoveableBox))
                    {
                        OnGrabBox(turnActionData.myMoveableBox);
                    }
                }
                break;

            default:
                break;
        }

        if (turnActionData.myConsumesTurn)
        {
            myTouchProgress.Reset();

            myTurnEvent.SignalDone();
            myTurnEvent = null;
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
            else if (touchEvent.myType == TouchEvent.Type.Tap)
            {
                Entity entity = FindEntityFromScreenClick(touchEvent.myTapPosition);
                if (entity != null && entity is MoveableBox moveableBox)
                {
                    return TurnActionData.CreateBox(moveableBox);
                }
            }
        }

        //Keyboard Input (for convenience)
        Direction moveDirection = Direction.Up;
        bool gotInput = false;

        if (Input.GetMouseButtonDown(0))
        {
            Entity entity = FindEntityFromScreenClick(Input.mousePosition);
            if (entity != null && entity is MoveableBox moveableBox)
            {
                return TurnActionData.CreateBox(moveableBox);
            }
        }

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

    private void HandleGrabbedMovement(Direction aMovementDirection)
    {
        switch (ComputeGrabbedMovementTypeInDirection(aMovementDirection))
        {
            case GrabbedMovementType.Push:
                if (myGrabbedBox.OnPlayerMoveBox(aMovementDirection))
                {
                    if (!Move(aMovementDirection))
                    {
                        // Player could not follow box => release box
                        // Ex. box triggered a breakable tile

                        if (myIsGrabbingBox)
                        {
                            OnReleaseBox();
                        }
                    }

                    PushAction(aMovementDirection, !myIsGrabbingBox);
                }
                break;
            case GrabbedMovementType.Pull:
                if (Move(aMovementDirection))
                {
                    if (!myGrabbedBox.OnPlayerMoveBox(aMovementDirection))
                    {
                        // Box could not follow player => release box
                        // Ex. we triggered a breakable tile

                        if (myIsGrabbingBox)
                        {
                            OnReleaseBox();
                        }
                    }

                    PullAction(aMovementDirection, !myIsGrabbingBox);
                }
                break;
            case GrabbedMovementType.Invalid:
            default:
                // TODO: Either disallow moving or move and drop the box.
                // Current behaviour is to ignore movement
                break;
        }
    }

    private void HandleNormalMovement(Direction aMovementDirection)
    {
        Entity entityAtNextPosition = GetEntityInDirection(aMovementDirection);
        InteractResult aInteractResult = InteractResult.Undefined;

        if (entityAtNextPosition != null)
        {
            if (entityAtNextPosition.GetType() == typeof(EnemyPath))
            {
                Kill(DeathReason.Enemy);
            }

            aInteractResult = entityAtNextPosition.Interact(this, aMovementDirection);
            
            if (aInteractResult == InteractResult.BoxMoved
                || aInteractResult == InteractResult.BoxMoveFailed)
            {
                KickAction(aMovementDirection);
            }
            
            else if (aInteractResult == InteractResult.KeyPickedUp
                || aInteractResult == InteractResult.Unlocked)
            {
                Move(aMovementDirection);
                MoveAction(aMovementDirection);
            }
            
            else
            {
                BlockedAction(aMovementDirection);
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
    }

    private bool CheckCanGrabBox(MoveableBox aMoveableBox)
    {
        Vector2Int toEntity = StageManager.ourInstance.GetEntityGridPosition(aMoveableBox) - StageManager.ourInstance.GetEntityGridPosition(this);
        return toEntity.sqrMagnitude == 1;
    }

    private GrabbedMovementType ComputeGrabbedMovementTypeInDirection(Direction aDirection)
    {
        Debug.Assert(myIsGrabbingBox, "ComputeGrabbedMovementType calle when not grabbing box!");

        Vector2Int toEntity = StageManager.ourInstance.GetEntityGridPosition(myGrabbedBox) - StageManager.ourInstance.GetEntityGridPosition(this);
        Vector2Int movementDisplacement = DirectionToVec(aDirection);

        if (movementDisplacement == toEntity)
        {
            return GrabbedMovementType.Push;
        }
        else if (movementDisplacement == -toEntity)
        {
            return GrabbedMovementType.Pull;
        }

        return GrabbedMovementType.Invalid;
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

    private void PullAction(Direction aMovementDirection, bool aBoxDropped)
    {
        if (aBoxDropped)
        {
            MoveAction(aMovementDirection);
        }
        else
        {
            myAnimator.Pull(ReverseDirection(aMovementDirection));
        }
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

    private void GrabAction(Direction aDirection)
    {
        myAnimator.Grab(aDirection);
    }

    private void LetGoAction()
    {
        myAnimator.LetGo();
    }
}
