using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerAnimator))]
public partial class Player : Entity
{
    public bool myIsInTurn => myTurnEvent != null;
    public bool myIsGrabbingBox => myGrabbedBox != null;

    [Header("Touch Settings")]
    [Tooltip("Length you must drag you finger accros the screen before the PLayer moves.")]
    [SerializeField] private float myDragDistance;
	
    [Tooltip("Longest time the player can swipe before the swipe becomes null (in seconds).")]
    [SerializeField] private float myMaxSwipeTime;
    [Tooltip("Minimum length of Swipe for it not to become null (in pixels).")]
    [SerializeField] private float myMinSwipeLength;

    private TurnEvent myTurnEvent = null;

    //Touch Related
    private float mySwipeStartTime;
    private float mySwipeEndTime;
    private Vector2 mySwipeStartPos;
    private Vector2 mySwipeEndPos;

    // Grabbable box
    private MoveableBox myGrabbedBox;

    private Camera myMainCamera;
    private PlayerAnimator myAnimator;

    public override void Action(TurnEvent aTurnEvent)
    {
        myTurnEvent = aTurnEvent;
        myAnimator.OnTurnAnimationFinish = EndTurn;
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
        myAnimator.OnTurnAnimationFinish = EndTurn;
    }

    private void Update()
    {
        // TODO: Remove
        if (base.IsDead())
        {
            StageManager.ourInstance.OnPlayerLoss();

            // Temporary while VictoryDefeatUI is not implemented
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        PlayerAction();
    }

    private void EndTurn()
    {
        Debug.Assert(myIsInTurn, "EndTurn called outside of Player's turn", this);
        myTurnEvent.SignalDone();
        myTurnEvent = null;
        myAnimator.OnTurnAnimationFinish = null;
    }

    private void OnGrabBox(MoveableBox aBox)
    {
        Debug.Assert(!myIsGrabbingBox, "OnGrabBox called when already grabbing box!");

        myGrabbedBox = aBox;
        myGrabbedBox.OnGrabbedByPlayer(this);
    }

    private void OnReleaseBox()
    {
        Debug.Assert(myIsGrabbingBox, "OnReleaseBox called when not grabbing box!");

        myGrabbedBox.OnReleasedByPlayer();
        myGrabbedBox = null;
    }

    private void PlayerAction()
    {
        if (myIsInTurn)
        {
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
                //EndTurn();
            }
        }
    }
	
    private TurnActionData GetTurnActionFromInput()
    {
        Direction moveDirection = Direction.Up;
        bool gotInput = false;

        //Touch Input
        // TODO: Touch input support for clicking on entities
        switch (TouchInput())
        {
            case 1: //Up
                gotInput = true;
                moveDirection = Direction.Up;
                break;
            case 2: //Down
                gotInput = true;
                moveDirection = Direction.Down;
                break;
            case 3: //Right
                gotInput = true;
                moveDirection = Direction.Right;
                break;
            case 4: //Left
                gotInput = true;
                moveDirection = Direction.Left;
                break;
            default:
                break;
        }

        //Keyboard Input (for convenience)
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

    private int TouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                mySwipeStartTime = Time.time;
                mySwipeStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                mySwipeEndTime = Time.time;
                mySwipeEndPos = touch.position;
                float swipeTime = mySwipeEndTime - mySwipeStartTime;
                float swipeLength = (mySwipeEndPos - mySwipeStartPos).magnitude;
                if (swipeTime < myMaxSwipeTime && swipeLength > myMinSwipeLength)
                {
                    Vector2 distance = mySwipeEndPos - mySwipeStartPos;
                    float xDistance = Mathf.Abs(distance.x);
                    float yDistance = Mathf.Abs(distance.y);
                    if ((xDistance > yDistance) && (touch.position.x > mySwipeStartPos.x))
                    {
                        return 3;
                    }
                    else if ((xDistance > yDistance) && (touch.position.x < mySwipeStartPos.x))
                    {
                        return 4;
                    }
                    else if ((yDistance > xDistance) && (touch.position.y > mySwipeStartPos.y))
                    {
                        return 1;
                    }
                    else if ((yDistance > xDistance) && (touch.position.y < mySwipeStartPos.y))
                    {
                        return 2;
                    }
                }
            }


        }
        return 0;
    }

    private void HandleGrabbedMovement(Direction aMovementDirection)
    {
        switch (ComputeGrabbedMovementTypeInDirection(aMovementDirection))
        {
            case GrabbedMovementType.Push:
                myGrabbedBox.OnPlayerMoveBox(aMovementDirection);
                Move(aMovementDirection);
                break;
            case GrabbedMovementType.Pull:
                Move(aMovementDirection);
                myGrabbedBox.OnPlayerMoveBox(aMovementDirection);
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
        Entity interactingEntity = GetEntityInDirection(aMovementDirection);

        if (interactingEntity != null)
        {
            interactingEntity.Interact(this, aMovementDirection);
        }
        else
        {
            Move(aMovementDirection);
            myAnimator.Hop();
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
}
