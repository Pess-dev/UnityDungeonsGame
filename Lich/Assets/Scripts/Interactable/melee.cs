using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Interactable
{
    public float damage = 1;

    private void Start()
    {
        canGrab = true;
    }

    protected override void Interact() 
    {
    }

    public override void Activate() 
    {
        active = true;
    }

    public override void Deactivate() 
    {
        active = false;
    }
}
