using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : Entity
{
    [SerializeField]
    private Direction[] mySteps;

    private int myStepsIndex;

    public override void Action(TurnEvent aTurnEvent)
    {
        Move(mySteps[myStepsIndex]);
        myStepsIndex++;

        if (myStepsIndex >= mySteps.Length)
        {
            myStepsIndex = 0;
            ReverseSteps();
        }

        aTurnEvent.SignalDone();
    }

    public override void Interact(Entity anEntity, Direction aDirection)
    {
        if (anEntity is Player)
        {
            //kill player
        }
    }

    private void ReverseSteps()
    {
        Direction[] newSteps = new Direction[mySteps.Length];

        for (int i = mySteps.Length - 1; i >= 0; i--)
        {
            Direction direction = Direction.Up;
            switch (mySteps[i])
            {
                case Direction.Up:
                    direction = Direction.Down;
                    break;
                case Direction.Right:
                    direction = Direction.Left;
                    break;
                case Direction.Down:
                    direction = Direction.Up;
                    break;
                case Direction.Left:
                    direction = Direction.Right;
                    break;
                default:
                    Debug.LogError(this + " has a directionless step, somehow.");
                    break;
            }

            newSteps[mySteps.Length - 1 - i] = direction;
        }

        mySteps = newSteps;
    }
}