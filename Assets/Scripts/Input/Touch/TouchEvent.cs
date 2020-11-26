using UnityEngine;

public struct TouchEvent
{
    public enum Type
    {
        None,

        Tap,
        Swipe
    }

    public static TouchEvent None => new TouchEvent
    {
        myType = Type.None,
    };

    public bool myIsValid => myType != Type.None;

    public Type myType;

    public Direction mySwipeDirection;

    public Vector2 myTapPosition;

    public static TouchEvent CreateSwipe(Direction aSwipeDirection)
    {
        return new TouchEvent
        {
            myType = Type.Swipe,

            mySwipeDirection = aSwipeDirection,
        };
    }

    public static TouchEvent CreateTap(Vector2 aTapPosition)
    {
        return new TouchEvent
        {
            myType = Type.Tap,

            myTapPosition = aTapPosition,
        };
    }

    public override string ToString()
    {
        return $"{{ Type: {myType}, Swipe Direction: {mySwipeDirection}, Tap Position: {myTapPosition} }}";
    }
}