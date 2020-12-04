﻿using UnityEngine;

public class EnemyPath : Entity
{
    [SerializeField]
    private Direction[] mySteps;
    [SerializeField]
    private bool myWillReverse = true;

    private int myStepsIndex;

    public override void Action(TurnEvent aTurnEvent)
    {
        if (mySteps.Length <= 0)
        {
            aTurnEvent.SignalDone();
            return;
        }

        Vector2Int newPosition = StageManager.ourInstance.GetEntityGridPosition(this) + DirectionToVec(mySteps[myStepsIndex]);
        if (!StageManager.ourInstance.IsPositionInGrid(newPosition))
        {
            aTurnEvent.SignalDone();
            return;
        }

        if (!StageManager.ourInstance.CanEntityMoveToPosition(this, newPosition))
        {
            Entity entity = StageManager.ourInstance.GetEntity(newPosition);
            if (entity is Player)
            {
                entity.Kill(DeathReason.Enemy);
            }
            aTurnEvent.SignalDone();
            return;
        }

        Tile tile = StageManager.ourInstance.GetTile(newPosition);
        if (tile is HoleTile holeTile && !holeTile.myIsFilled)
        {
            aTurnEvent.SignalDone();
            return;
        }

        Move(mySteps[myStepsIndex]);
        myStepsIndex++;

        if (myStepsIndex >= mySteps.Length)
        {
            myStepsIndex = 0;
            if (myWillReverse)
            {
                ReverseSteps();
            }
        }

        aTurnEvent.SignalDone();
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