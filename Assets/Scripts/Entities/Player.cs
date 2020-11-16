using UnityEngine;

public class Player : Entity
{
    [Header("Objects")]
    [Tooltip("The Player GameObject")]
    [SerializeField] private GameObject myPlayer;
    Direction myMoveDirection;
    bool myPlayerInputted;



    public void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            myMoveDirection = Direction.Up;
        }
    }

    public override void Action(TurnEvent aTurnEvent)
    {
        if (myPlayerInputted)
        {
            Vector2Int myCurrentPosition = StageManager.ourInstance.GetTilePositionFromWorld(transform.position);
            Vector2Int newPosition = Vector2Int.zero;

            switch (myMoveDirection)
            {
                case Direction.Up:
                    newPosition = new Vector2Int(myCurrentPosition.x + 1, myCurrentPosition.y);


                    break;
                case Direction.Right:
                    newPosition = new Vector2Int(myCurrentPosition.x, myCurrentPosition.y + 1);

                    break;
                case Direction.Down:
                    newPosition = new Vector2Int(myCurrentPosition.x - 1, myCurrentPosition.y);

                    break;
                case Direction.Left:
                    newPosition = new Vector2Int(myCurrentPosition.x, myCurrentPosition.y - 1);
                    break;
                default:
                    break;
            }

            StageManager.ourInstance.MoveEntity(this, newPosition);


        }

    }

}
