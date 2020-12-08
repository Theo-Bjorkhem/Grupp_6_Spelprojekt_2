using UnityEngine;

[System.Serializable]
public struct TouchConfiguration
{
    public static TouchConfiguration Default => new TouchConfiguration
    {
        mySwipeMinDistance = 100.0f,
        mySwipeMaxDuration = 0.5f,
    };

    [Header("Swipe Configuration")]

    [Tooltip("Min swipe distance to count as a swipe (in pixels).")]
    public float mySwipeMinDistance;

    [Tooltip("Max swipe duration to count as a swipe (in seconds).")]
    public float mySwipeMaxDuration;
}