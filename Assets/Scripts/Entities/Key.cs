using UnityEngine;

[RequireComponent(typeof(KeyAnimator))]
public class Key : Entity
{
    private KeyAnimator myKeyAnimator;

    public override InteractResult Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);
        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("PickupKey");
        }

        ++StageManager.ourInstance.myKeyCount;

        StageManager.ourInstance.UnregisterEntity(this);

        myKeyAnimator.OnPickedUp(() => gameObject.SetActive(false));
		
        return InteractResult.KeyPickedUp;
    }

    private void Awake()
    {
        myKeyAnimator = GetComponent<KeyAnimator>();
    }
}
