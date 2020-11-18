public class FreezableValue<T> where T : class
{
    public T myValue
    {
        get
        {
            return myIsFrozen ? myFrozenValue : myMainValue;
        }
    }

    private T myMainValue;
    private T myFrozenValue;

    private bool myIsFrozen = false;
    private bool myIsDirty = true;

    public FreezableValue(T aValue)
    {
        myMainValue = aValue;
    }

    public void Set(T aNewValue)
    {
        myMainValue = aNewValue;
        myIsDirty = true;
    }

    public void Freeze()
    {
        if (myIsDirty)
        {
            myFrozenValue = myMainValue;

            myIsDirty = false;
        }

        myIsFrozen = true;
    }

    public void Unfreeze()
    {
        myIsFrozen = false;
    }
}
