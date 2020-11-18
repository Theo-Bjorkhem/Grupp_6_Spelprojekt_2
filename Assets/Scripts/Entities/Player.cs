using UnityEngine;

public class Player : Entity
{
    [Header("Objects")]
    [Tooltip("The Player GameObject.")]
    [SerializeField] private GameObject myPlayer;

    [Header("Touch Settings")]
    [Tooltip("Length you must drag you finger accros the screen before the PLayer moves.")]
    [SerializeField] private float myDragDistance;

    bool myPlayerInputted = false;
    TurnEvent myTurnEvent = null;
    Vector2Int myCurrentPosition;
    Vector2Int myMoveTilePos;

    private Vector2 myOldTouchPos;

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

}
