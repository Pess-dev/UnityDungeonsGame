using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class melee : Interactable
{
    [SerializeField]
    private Transform grip;

    public float damage = 1;

    private bool grabbed;

    private void Start()
    {
        grabbed = false;
        canGrab = true;
    }

    protected override void Interact() 
    {
        grabbed = !grabbed;
    }

    public void Release()
    {
        grabbed = false;
    }
}
