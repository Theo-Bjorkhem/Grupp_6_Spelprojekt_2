using UnityEngine;

public class Player : Entity
{
    private TurnEvent myTurnEvent = null;
	
    [Header("Objects")]
    [Tooltip("The Player GameObject.")]
    [SerializeField] private GameObject myPlayer;

    [Header("Touch Settings")]
    [Tooltip("Length you must drag you finger accros the screen before the PLayer moves.")]
    [SerializeField] private float myDragDistance;

    private Vector2 myOldTouchPos;

    public void Update()
    {
        PlayerAction();
    }
	
    private void PlayerAction()
    {
        if (myTurnEvent != null)
        {
            Direction moveDirection = Direction.Up;
            bool gotInput = false;

            //Touch Input
            switch (TouchInput())
            {
                case 1: //Up
                    gotInput = true;
					moveDirection = Direction.Right;
                    break;
                case 2: //Down
                    gotInput = true;
					moveDirection = Direction.Left;
                    break;
                case 3: //Right
                    gotInput = true;
					moveDirection = Direction.Down;
                    break;
                case 4: //Left
                    gotInput = true;
					moveDirection = Direction.Up;
                    break;
                default:
                    break;
            }
			
            //Keyboard Input (for convenience)
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                gotInput = true;
				moveDirection = Direction.Right;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
				gotInput = true;
				moveDirection = Direction.Down;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
				gotInput = true;
				moveDirection = Direction.Up;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
				gotInput = true;
				moveDirection = Direction.Left;
            }

            if (gotInput)
            {
                Entity interactingEntity = GetEntityInDirection(moveDirection);

                if (interactingEntity != null)
                {
                    interactingEntity.Interact(this, moveDirection);
                }
                else
                {
                    Move(moveDirection);

                }
                myTurnEvent.SignalDone();
                myTurnEvent = null;
            }
        }
    }
	
    private int TouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Debug.Log("Touch Began");
                    myOldTouchPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    Debug.Log("Touch Moved");
                    Vector2 newTouchPos = touch.position;

                    if ((newTouchPos.x - myOldTouchPos.x) > myDragDistance)
                    {
                        Debug.Log("Returned 3");
                        return 3;
                    }
                    else if (( myOldTouchPos.x - newTouchPos.x) > -myDragDistance)
                    {
                        Debug.Log("Returned 4");
                        return 4;
                    }

                    if ((newTouchPos.y - myOldTouchPos.y) > myDragDistance)
                    {
                        Debug.Log("Returned 1");
                        return 1;
                    }
                    else if ((myOldTouchPos.y - newTouchPos.y) > -myDragDistance)
                    {
                        Debug.Log("Returned 2");
                        return 2;
                    }

                    break;
                case TouchPhase.Ended:
                    Debug.Log("Touch Ended");
                    break;
                default:
                    break;
            }
        }
        return 0;
    }
	
    public override void Action(TurnEvent aTurnEvent)
    {
        myTurnEvent = aTurnEvent;
    }
	
    private Entity GetEntityInDirection(Direction aDirection)
    {
        Vector2Int myPosition = StageManager.ourInstance.GetTilePositionFromWorldEntity(transform.position);
        Vector2Int checkPosition;

        switch (aDirection)
        {
            case Direction.Up:
               checkPosition = (myPosition + new Vector2Int(0, 1));
                break;
            case Direction.Right:
                checkPosition = (myPosition + new Vector2Int(1, 0));
                break;
            case Direction.Down:
                checkPosition = (myPosition + new Vector2Int(0, -1));
                break;
            case Direction.Left:
                checkPosition = (myPosition + new Vector2Int(-1, 0));
                break;
            default:
                return null;
        }

        if(checkPosition.x >= 0 && checkPosition.x < StageManager.ourInstance.myGridWidth && checkPosition.y >= 0 && checkPosition.y < StageManager.ourInstance.myGridHeight)
        {
            return StageManager.ourInstance.GetEntity(checkPosition);

        }
        else
        {
            return null;
        }
    }
}
