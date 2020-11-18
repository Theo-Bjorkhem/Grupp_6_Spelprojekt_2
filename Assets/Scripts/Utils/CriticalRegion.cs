using UnityEngine;

/// <summary>
/// OBS! This has nothing do wo with thread safety.
/// It is only used to ensure two regions that acquire the lock cannot execute before one releases the lock.
/// Mostly to keep our sanity if a chain of functions inside the lock calls another function that needs the lock.
/// </summary>
public class CriticalRegion
{
    public bool myIsLocked { get; private set; } = false;

    public void Lock()
    {
        Debug.Assert(!myIsLocked, "CriticalRegion entered while locked!");

        if (myIsLocked)
        {
            throw new System.InvalidOperationException("CriticalRegion entered while locked!");
        }

        myIsLocked = true;
    }

    public void Unlock()
    {
        Debug.Assert(myIsLocked, "Logic error, CriticalRegion unlock called while not locked!");

        if (!myIsLocked)
        {
            throw new System.InvalidOperationException("CriticalRegion unlocked while not locked!");
        }

        myIsLocked = false;
    }
}