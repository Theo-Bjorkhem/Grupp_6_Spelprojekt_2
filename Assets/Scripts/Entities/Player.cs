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
                myMoveTilePos = myCurrentPosition + new Vector2Int(1, 0);
                myPlayerInputted = true;

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                myMoveTilePos = myCurrentPosition + new Vector2Int(0, -1);
                myPlayerInputted = true;

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                myMoveTilePos = myCurrentPosition + new Vector2Int(0, 1);
                myPlayerInputted = true;

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                myMoveTilePos = myCurrentPosition + new Vector2Int(-1, 0);
                myPlayerInputted = true;

            }
            if (myPlayerInputted == true)
            {
                StageManager.ourInstance.MoveEntity(this, myMoveTilePos);
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
