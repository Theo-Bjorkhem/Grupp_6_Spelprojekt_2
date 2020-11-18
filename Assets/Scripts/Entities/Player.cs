using UnityEngine;

public class Player : Entity
{
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
    //Turn Related
    bool myPlayerInputted = false;
    TurnEvent myTurnEvent = null;
    Vector2Int myCurrentPosition;
    Vector2Int myMoveTilePos;
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
        myCurrentPosition = StageManager.ourInstance.GetTilePositionFromWorld(transform.position);
        if (myTurnEvent != null)
        {
            //Touch Input
            switch (TouchInput())
            {
                case 1: //Up
                    myPlayerInputted = true;
                    myMoveTilePos = myCurrentPosition + new Vector2Int(1, 0);
                    break;
                case 2: //Down
                    myPlayerInputted = true;
                    myMoveTilePos = myCurrentPosition + new Vector2Int(-1, 0);
                    break;
                case 3: //Right
                    myPlayerInputted = true;
                    myMoveTilePos = myCurrentPosition + new Vector2Int(0, -1);
                    break;
                case 4: //Left
                    myPlayerInputted = true;
                    myMoveTilePos = myCurrentPosition + new Vector2Int(0, 1);
                    break;
                default:
                    break;
            }
            //Keyboard Input (for convenience)
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                myPlayerInputted = true;
                myMoveTilePos = myCurrentPosition + new Vector2Int(1, 0);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                myPlayerInputted = true;
                myMoveTilePos = myCurrentPosition + new Vector2Int(0, -1);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                myPlayerInputted = true;
                myMoveTilePos = myCurrentPosition + new Vector2Int(0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                myPlayerInputted = true;
                myMoveTilePos = myCurrentPosition + new Vector2Int(-1, 0);
            }
            //End Turn
            if (myPlayerInputted == true)
            {
                if (StageManager.ourInstance.CanEntityMoveToPosition(this, myMoveTilePos))
                {
                    StageManager.ourInstance.MoveEntity(this, myMoveTilePos);

                    //TODO: Replace with actual animation
                    transform.position = new Vector3(myMoveTilePos.x, 0.5f, myMoveTilePos.y);
                }
                myTurnEvent.SignalDone();
                myTurnEvent = null;
                myPlayerInputted = false;
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

}
