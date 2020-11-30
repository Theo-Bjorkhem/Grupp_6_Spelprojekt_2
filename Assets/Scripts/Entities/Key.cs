using UnityEngine;

[RequireComponent(typeof(KeyAnimator))]
public class Key : Entity
{
    private KeyAnimator myKeyAnimator;

    public override InteractResult Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);
        AudioManager.ourInstance.PlaySound("PickupKey");

        StageManager.ourInstance.myHasKey = true;

        StageManager.ourInstance.UnregisterEntity(this);

        myKeyAnimator.OnPickedUp(() => gameObject.SetActive(false));
        // TODO: SFX
        // AudioManager.ourInstance.PlaySound("Key_Pickup");
		
        return InteractResult.KeyPickedUp;
    }

    private void Awake()
    {
        myKeyAnimator = GetComponent<KeyAnimator>();
    }
}
