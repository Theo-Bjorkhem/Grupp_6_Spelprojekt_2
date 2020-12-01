public static class EasingImplementations
{

    public static float Ease(EasingType aType, float aTime)
    {
        switch(aType)
        {
            case EasingType.Linear:
            default:
                return Linear(aTime);
        }
    }

    public static float Linear(float aTime)
    {
        return aTime;
    }

}
