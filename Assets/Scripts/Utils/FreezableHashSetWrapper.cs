using System.Collections.Generic;

public class FreezableHashSetWrapper<T>
{
    public HashSet<T> myValue
    {
        get => myIsFrozen ? myFrozenValue : myMainValue;
    }

    private readonly HashSet<T> myMainValue = new HashSet<T>();
    private readonly HashSet<T> myFrozenValue = new HashSet<T>();

    private bool myIsFrozen = false;
    private bool myIsDirty = true;

    public void Add(T anItem)
    {
        myMainValue.Add(anItem);
        myIsDirty = true;
    }

    public void Remove(T anItem)
    {
        myMainValue.Remove(anItem);
        myIsDirty = true;
    }

    public void Freeze()
    {
        if (myIsDirty)
        {
            myFrozenValue.Clear();

            myFrozenValue.UnionWith(myMainValue);

            myIsDirty = false;
        }

        myIsFrozen = true;
    }

    public void Unfreeze()
    {
        myIsFrozen = false;
    }
}
