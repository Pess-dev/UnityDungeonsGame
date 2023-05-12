using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public bool opened = false;
    public bool deactivateAfterUse = true;

    [SerializeField]
    private Animator anim;

    private void Start()
    {
    }

    public override void Interact(Unit user = null)
    {
        base.Interact(user);

        opened = !opened;

        if (deactivateAfterUse)
            Deactivate();

        if (anim != null)
            anim.SetBool("opened", opened);
    }
}
