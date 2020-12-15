using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class MoveableBoxAnimator : MonoBehaviour
{
    public CameraShake_script myCameraShake;

    private struct ActionData
    {
        public IEnumerator myCoroutine;
        public System.Action myInstaCompleteAction;
    }

    public PlayerAnimator myPlayerAnimator { get; set; }

    [SerializeField]
    private AnimationCurve myKickAnimationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [SerializeField]
    private AnimationCurve myFallAnimationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [SerializeField]
    private float myKickDuration = 0.5f;

    [SerializeField]
    private float myFallDuration = 0.35f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float myFallDepth = 0.8f;

    [SerializeField]
    private VisualEffect myFallDust;

    private System.Collections.Generic.Queue<ActionData> myActionQueue = new System.Collections.Generic.Queue<ActionData>(2);
    
    public void PreparePossibleMove()
    {
        CompleteAllActions();
    }

    public void DoMove(Vector3 aFrom, Vector3 aTo)
    {
        Debug.Assert(myPlayerAnimator != null, "Animating box move without player!");

        QueueAction(DoMoveCo(aFrom, aTo), () => transform.position = aTo);
    }

    public void DoFall()
    {
        QueueAction(DoFallCo(), () => transform.position = new Vector3(transform.position.x, StageManager.ourInstance.myTileSize * -myFallDepth, transform.position.z));
    }

    private IEnumerator DoMoveCo(Vector3 aFrom, Vector3 aTo)
    {
        transform.position = aFrom;

        // Wait until player is in pushing or kicking animation
        while(true)
        {
            if (myPlayerAnimator.IsInState("Push"))
            {
                Transform positionRoot = myPlayerAnimator.GetAnimationPositionRoot();

                while (myPlayerAnimator.IsInState("Push"))
                {
                    float time = Mathf.Clamp01(1.0f + positionRoot.localPosition.z);

                    transform.position = Vector3.Lerp(aFrom, aTo, time);

                    yield return null;
                }

                transform.position = aTo;

                yield break;
            }

            if (myPlayerAnimator.IsInState("Kick"))
            {
                float elapsed = 0.0f;

                while (elapsed <= myKickDuration)
                {
                    elapsed += Time.deltaTime;

                    float time = Mathf.Clamp01(elapsed / myKickDuration);

                    transform.position = Vector3.Lerp(aFrom, aTo, myKickAnimationCurve.Evaluate(time));

                    yield return null;
                }

                transform.position = aTo;

                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator DoFallCo()
    {
        yield return null; // Don't do anything immediately when queued

        if (myFallDust != null)
        {
            myFallDust.transform.SetParent(null, true);
            myFallDust.Play();
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(startPos.x, StageManager.ourInstance.myTileSize * -myFallDepth, startPos.z);

        float elapsed = 0.0f;

        while (elapsed <= myFallDuration)
        {
            elapsed += Time.deltaTime;

            float time = Mathf.Clamp01(elapsed / myFallDuration);

            transform.position = Vector3.Lerp(startPos, endPos, myFallAnimationCurve.Evaluate(time));

            yield return null;
        }

        transform.position = endPos;

        //myCameraShake.ShakeCamera();
    }

    private void CompleteAllActions()
    {
        while (myActionQueue.Count > 0)
        {
            myActionQueue.Dequeue().myInstaCompleteAction();
        }
    }

    private void QueueAction(IEnumerator aCoroutine, System.Action aInstaCompleteAction)
    {
        if (aCoroutine.MoveNext())
        {
            myActionQueue.Enqueue(new ActionData
            {
                myCoroutine = aCoroutine,
                myInstaCompleteAction = aInstaCompleteAction,
            });
        }
    }

    private void Update()
    {
        if (myActionQueue.Count == 0)
            return;

        ActionData actionData = myActionQueue.Peek();

        if (!actionData.myCoroutine.MoveNext())
        {
            myActionQueue.Dequeue();
        }
    }
}
