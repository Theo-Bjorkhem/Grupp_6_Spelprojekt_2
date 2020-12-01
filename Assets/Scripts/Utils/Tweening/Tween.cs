using UnityEngine;

public class Tween : CustomYieldInstruction
{
    public override bool keepWaiting => !myIsComplete;

    public bool myIsComplete => myTime >= 1.0f;
    public float myTime => Mathf.Clamp01(myElapsedTime / myDuration);

    public TweenManager.TweenUpdateCallback myTweenUpdateCallback;
    public TweenManager.TweenCompleteCallback myTweenCompleteCallback;

    public float myDuration;
    public EasingType myEasingType;
    
    private float myElapsedTime;

    public Tween SetOnCompleteCallback(TweenManager.TweenCompleteCallback aTweenCompleteCallback)
    {
        myTweenCompleteCallback = aTweenCompleteCallback;
        return this;
    }

    public void Complete(bool aTriggerUpdateCallback)
    {
        myElapsedTime = myDuration;

        if (aTriggerUpdateCallback)
        {
            TriggerUpdateCallback();
        }
    }

    public void Update(float aDeltaTime)
    {
        if (myIsComplete)
            return;

        myElapsedTime += aDeltaTime;

        TriggerUpdateCallback();
    }

    private void TriggerUpdateCallback()
    {
        myTweenUpdateCallback(EasingImplementations.Ease(myEasingType, myTime));
    }
}
