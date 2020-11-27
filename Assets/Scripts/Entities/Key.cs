using UnityEngine;

[RequireComponent(typeof(KeyAnimator))]
public class Key : Entity
{
    private KeyAnimator myKeyAnimator;

    public override void Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);

        StageManager.ourInstance.myHasKey = true;

        StageManager.ourInstance.UnregisterEntity(this);

        myKeyAnimator.OnPickedUp(() => gameObject.SetActive(false));
        // TODO: SFX
        // AudioManager.ourInstance.PlaySound("Key_Pickup");
    }

    private void Awake()
    {
        myKeyAnimator = GetComponent<KeyAnimator>();
    }
}
