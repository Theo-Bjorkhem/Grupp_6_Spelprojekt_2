using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBox : Entity
{
    // Update is called once per frame
    void Update()
    {

    }

    public override void Interact(Entity anEntity, Direction aDirection)
    {
        base.Interact(anEntity, aDirection);       
        Move(aDirection);
    }

}
