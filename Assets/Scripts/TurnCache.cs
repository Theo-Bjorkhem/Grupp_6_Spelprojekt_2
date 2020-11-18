using System.Collections.Generic;
using UnityEngine;

public class TurnCache
{
    private Queue<TurnEvent> myTurnEvents;

    public TurnCache(int aCount)
    {
        Allocate(aCount);
    }

    public TurnEvent Next()
    {
        if (myTurnEvents.Count == 0)
        {
#if UNITY_EDITOR
            Debug.LogWarning("TurnCache had to allocate more turn events!");
#endif

            return new TurnEvent();
        }

        TurnEvent turnEvent = myTurnEvents.Dequeue();
        Reset(turnEvent);

        return turnEvent;
    }

    public void Recycle(TurnEvent anEvent)
    {
        myTurnEvents.Enqueue(anEvent);
    }

    private void Allocate(int aCount)
    {
        myTurnEvents = new Queue<TurnEvent>(aCount);

        for (int i = 0; i < aCount; ++i)
        {
            myTurnEvents.Enqueue(new TurnEvent());
        }
    }

    private void Reset(TurnEvent anEvent) => anEvent.ResetState();
}
