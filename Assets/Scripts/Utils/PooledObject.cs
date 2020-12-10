using System.Collections.Generic;
using UnityEngine;

public abstract class PooledObject<T, R> where T : new ()
{
    protected R myRoot;
    protected Queue<T> myQueue;

    public PooledObject(R aRoot, int aCapacity)
    {
        myRoot = aRoot;
        myQueue = new Queue<T>(aCapacity);

        for (int i = 0; i < aCapacity; ++i)
        {
            myQueue.Enqueue(PerformCreate());
        }
    }

    public T GetNext()
    {
        if (myQueue.Count == 0)
        {
#if UNITY_EDITOR

            Debug.LogWarning("Not enough pooled objects => allocating more!");

#endif

            return PerformCreate();
        }

        return myQueue.Dequeue();
    }

    public void Recycle(T anObject)
    {
        PerformRecycle(anObject);
        myQueue.Enqueue(anObject);
    }

    public void Clear()
    {
        while (myQueue.Count > 0)
        {
            PerformDelete(myQueue.Dequeue());
        }
    }

    protected virtual void PerformDelete(T anObject) { }

    protected virtual T PerformCreate() => new T();
    protected virtual void PerformRecycle(T anObject) { }
}