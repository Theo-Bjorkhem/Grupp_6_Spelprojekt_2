using UnityEngine;

public class Player : Entity
{
    [Header("Objects")]
    [Tooltip("The Player GameObject")]
    [SerializeField] private GameObject myPlayer;
    bool myPlayerInputted = false;
    TurnEvent myTurnEvent = null;
    Vector2Int myCurrentPosition;
    Vector2Int myMoveTilePos;

    public void Update()
    {
        myCurrentPosition = StageManager.ourInstance.GetTilePositionFromWorld(transform.position);

        if (myTurnEvent != null)
        {
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
            if (myPlayerInputted == true)
            {
                StageManager.ourInstance.MoveEntity(this, myMoveTilePos);

                //TODO: Replace with actual animation
                transform.position = new Vector3(myMoveTilePos.x, 0.5f, myMoveTilePos.y);

                myTurnEvent.SignalDone();
                myTurnEvent = null;
                myPlayerInputted = false;
            }
        }
    }

    public override void Action(TurnEvent aTurnEvent)
    {
        myTurnEvent = aTurnEvent;

    }

}
