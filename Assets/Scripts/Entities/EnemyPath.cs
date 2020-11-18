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
        if (!StageManager.ourInstance.CanEntityMoveToPosition(this, StageManager.ourInstance.GetEntityGridPosition(this) + DirectionToVec(mySteps[myStepsIndex])))
        {
            aTurnEvent.SignalDone();
            return;
        }

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
            Direction direction = ReverseDirection(mySteps[i]);
            newSteps[mySteps.Length - 1 - i] = direction;
        }

        mySteps = newSteps;
    }
}