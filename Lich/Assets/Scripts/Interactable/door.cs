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
        canDrag = false;
    }

    public override void Interact()
    {
        opened = !opened;

        Deactivate();

        if (anim != null)
            anim.SetBool("opened", opened);
    }
}
