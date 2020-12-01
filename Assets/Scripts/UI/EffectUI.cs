using UnityEngine;
using UnityEngine.UI;

public class EffectUI : MonoBehaviour
{
    public static EffectUI ourInstance;

    [SerializeField]
    private Image myFadeImage;

    private Tween myTween;

    public Tween FadeOut(float aDuration)
    {
        EnsureNotTweening();

        myFadeImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        myFadeImage.gameObject.SetActive(true);
        myTween = TweenManager.DoTween(aDuration, EasingType.Linear, (float aTime) =>
        {
            Color color = myFadeImage.color;
            color.a = aTime;
            myFadeImage.color = color;
        });

        return myTween;
    }

    public Tween FadeIn(float aDuration)
    {
        EnsureNotTweening();

        myFadeImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        myTween = TweenManager.DoTween(aDuration, EasingType.Linear, (float aTime) =>
        {
            Color color = myFadeImage.color;
            color.a = 1.0f - aTime;
            myFadeImage.color = color;
        }).SetOnCompleteCallback(() => myFadeImage.gameObject.SetActive(false));

        return myTween;
    }

    private void EnsureNotTweening()
    {
        if (myTween != null)
        {
            myTween.Complete(true);
            myTween = null;
        }
    }

    private void Start()
    {
        // Reset
        myFadeImage.gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (ourInstance != null)
        {
            Debug.LogError("Multiple EffectUI in scene => destroying newest!");

            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        ourInstance = this;
    }
}
