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
	
    [Tooltip("Longest time the player can swipe before the swipe becomes null (in seconds).")]
    [SerializeField] private float myMaxSwipeTime;
    [Tooltip("Minimum length of Swipe for it not to become null (in pixels).")]
    [SerializeField] private float myMinSwipeLength;
	
    //Touch Related
    private float mySwipeStartTime;
    private float mySwipeEndTime;
    private Vector2 mySwipeStartPos;
    private Vector2 mySwipeEndPos;

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
            if (touch.phase == TouchPhase.Began)
            {
                mySwipeStartTime = Time.time;
                mySwipeStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                mySwipeEndTime = Time.time;
                mySwipeEndPos = touch.position;

            }

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
