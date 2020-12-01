using System.Collections.Generic;
using UnityEngine;

public class TweenManager : MonoBehaviour
{
    public delegate void TweenUpdateCallback(float aTime);
    public delegate void TweenCompleteCallback();

    public static TweenManager ourInstance
    {
        get
        {
            if (ourTweenManager == null)
            {
                ourTweenManager = CreateInstance();
            }

            return ourTweenManager;
        }
    }

    private static TweenManager ourTweenManager;

    private List<Tween> myNormalUpdateTweens = new List<Tween>();
    private List<Tween> myLateUpdateTweens = new List<Tween>();
    private List<Tween> myFixedUpdateTweens = new List<Tween>();

    /// <summary>
    /// Can be used to create the tween manager ahead of time to prevent stutter.
    /// </summary>
    public static void Init()
    {
        if (ourInstance == null)
        {
            Debug.LogError("TweenManager init failed!");
        }
    }

    public static Tween DoTween(float aDuration, EasingType anEasingType, TweenUpdateCallback aTweenCallback, UpdateType anUpdateType = UpdateType.Normal)
    {
        Tween tween = new Tween()
        {
            myDuration = aDuration,
            myEasingType = anEasingType,
            myTweenUpdateCallback = aTweenCallback,
        };

        GetTweenList(anUpdateType).Add(tween);

        return tween;
    }

    private static List<Tween> GetTweenList(UpdateType anUpdateType)
    {
        switch (anUpdateType)
        {
            case UpdateType.Normal:
            default:
                return ourInstance.myNormalUpdateTweens;

            case UpdateType.Late:
                return ourInstance.myLateUpdateTweens;

            case UpdateType.Fixed:
                return ourInstance.myFixedUpdateTweens;
        }
    }

    private static TweenManager CreateInstance()
    {
        GameObject root = new GameObject("TweenManager");

        DontDestroyOnLoad(root);

        return root.AddComponent<TweenManager>();
    }

    private static void RunTweens(List<Tween> aTweenList)
    {
        for (int i = 0; i < aTweenList.Count; ++i)
        {
            Tween tween = aTweenList[i];

            tween.Update(Time.deltaTime);

            if (tween.myIsComplete)
            {
                tween.myTweenCompleteCallback?.Invoke();

                aTweenList.RemoveAt(i);
                --i;
            }
        }
    }
    
    private void Update()
    {
        RunTweens(GetTweenList(UpdateType.Normal));
    }

    private void LateUpdate()
    {
        RunTweens(GetTweenList(UpdateType.Late));
    }

    private void FixedUpdate()
    {
        RunTweens(GetTweenList(UpdateType.Fixed));
    }
}
