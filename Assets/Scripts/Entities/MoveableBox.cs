using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBox : Entity
{
    // Update is called once per frame
    void Update()
    {

    }

    public void Push(Direction aDirection)
    {
        Move(aDirection);
    }
}
