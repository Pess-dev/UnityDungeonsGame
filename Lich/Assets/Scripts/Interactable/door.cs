using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    bool opened = false;

    [SerializeField]
    private Animator anim;

    private void Start()
    { 
        canGrab = false;
    }

    protected override void Interact()
    {
        opened = !opened;

        active = false;

        if (anim != null)
            anim.SetBool("opened", opened);
    }
}
