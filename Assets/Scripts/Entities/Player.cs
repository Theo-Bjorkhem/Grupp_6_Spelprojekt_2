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
                Move(moveDirection);
                
                myTurnEvent.SignalDone();
                myTurnEvent = null;
            }
        }
    }

    public override void Action(TurnEvent aTurnEvent)
    {
        myTurnEvent = aTurnEvent;
    }
}
