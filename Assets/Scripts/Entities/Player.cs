using UnityEngine;

public class Player : Entity
{
    private TurnEvent myTurnEvent = null;

    public void Update()
    {
        if (myTurnEvent != null)
        {
            Direction moveDirection = Direction.Up;
            bool gotInput = false;

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
