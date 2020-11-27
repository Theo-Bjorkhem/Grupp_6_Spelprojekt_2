using UnityEngine;

public class KeyAnimator : MonoBehaviour
{
    private Animator myAnimator;

    private System.Action myKeyFadedCallback;

    public void OnPickedUp(System.Action aKeyFadedCallback)
    {
        myKeyFadedCallback = aKeyFadedCallback;

        myAnimator.SetTrigger("PickedUp");
    }

    /// <summary>
    /// Called from Animation Event when key is fully faded.
    /// </summary>
    private void OnKeyFaded()
    {
        myKeyFadedCallback?.Invoke();
        myKeyFadedCallback = null;
    }

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }
}
