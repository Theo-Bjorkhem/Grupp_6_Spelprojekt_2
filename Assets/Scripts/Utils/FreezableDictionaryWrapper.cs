using System.Collections.Generic;

public class FreezableDictionaryWrapper<K, V>
{
    public Dictionary<K, V> myValue
    {
        get => myIsFrozen ? myFrozenValue : myMainValue;
    }

    private readonly Dictionary<K, V> myMainValue = new Dictionary<K, V>();
    private readonly Dictionary<K, V> myFrozenValue = new Dictionary<K, V>();

    private bool myIsFrozen = false;
    private bool myIsDirty = true;

    public void Add(K aKey, V aValue)
    {
        myMainValue.Add(aKey, aValue);
        myIsDirty = true;
    }

    public void Remove(K aKey)
    {
        myMainValue.Remove(aKey);
        myIsDirty = true;
    }

    public void Freeze()
    {
        if (myIsDirty)
        {
            myFrozenValue.Clear();

            foreach (KeyValuePair<K, V> kvp in myMainValue)
            {
                myFrozenValue.Add(kvp.Key, kvp.Value);
            }

            myIsDirty = false;
        }

        myIsFrozen = true;
    }

    public void Unfreeze()
    {
        myIsFrozen = false;
    }
}
