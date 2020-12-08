using UnityEngine;

public class TouchProgress
{
    public bool myHasCompleteEvent => myTouchEvent.myIsValid;

    public TouchEvent myTouchEvent { get; private set; } = TouchEvent.None;

    public TouchConfiguration myTouchConfiguration { get; set; } = TouchConfiguration.Default;

    private float myTouchStartTime = -1.0f;
    private Vector2 myTouchStartPos = Vector2.zero;

    /// <summary>
    /// Should be called once per frame until <see cref="myHasCompleteEvent"/> is true. After which a complete TurnEvent can be retrieved via <see cref="myTouchEvent"/>.
    /// To cancel ongoing touch recognition call <see cref="Reset"/>.
    /// </summary>
    public void Update()
    {
        if (myHasCompleteEvent)
        {
            Reset();
        }

        if (Input.touchCount == 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                myTouchStartTime = Time.time;
                myTouchStartPos = touch.position;
                break;

            case TouchPhase.Moved:
                if (myTouchStartTime < 0.0f)
                {
                    break;
                }

                float duration = Time.time - myTouchStartTime;

                Vector2 swipeDelta = touch.position - myTouchStartPos;
                float swipeDistance = swipeDelta.magnitude;

                if (duration < myTouchConfiguration.mySwipeMaxDuration
                    && swipeDistance >= myTouchConfiguration.mySwipeMinDistance)
                {
                    // Swipe
                    Direction swipeDirection;

                    if (Mathf.Abs(swipeDelta.y) >= Mathf.Abs(swipeDelta.x))
                    {
                        swipeDirection = swipeDelta.y >= 0.0f ? Direction.Up : Direction.Down;
                    }
                    else
                    {
                        swipeDirection = swipeDelta.x >= 0.0f ? Direction.Right : Direction.Left;
                    }

                    myTouchEvent = TouchEvent.CreateSwipe(swipeDirection);
                }
                break;

            case TouchPhase.Ended:
                ResetTrackingState();
                break;

            case TouchPhase.Canceled:
                ResetTrackingState();
                break;
        }
    }

    public void Reset()
    {
        ResetTrackingState();

        myTouchEvent = TouchEvent.None;
    }

    private void ResetTrackingState()
    {
        myTouchStartTime = -1.0f;
        myTouchStartPos = Vector2.zero;
    }
}
