using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public bool opened = false;
    public bool deactivateAfterUse = true;

    public string lockedPrompt = "...";
    public string unlockedPrompt = "!";

    [SerializeField]
    private Animator anim;

    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.white;
    
    [SerializeField]
    private bool locked = true;

    private void Start()
    {
        SetLocked(locked);
        if (anim != null)
            anim.SetBool("opened", opened);
    }

    public void SetLocked(bool status) 
    {
        locked = status;
        if (locked)
        {
            outline.OutlineColor = lockedColor;
            promptMessage = lockedPrompt;
        }
        else
        {
            outline.OutlineColor = unlockedColor;
            promptMessage = unlockedPrompt;
        }
    }

    public override void Interact(Unit user = null)
    {
        base.Interact(user);

        if (locked) 
            return;

        opened = !opened;

        if (deactivateAfterUse)
            Deactivate();

        if (anim != null)
            anim.SetBool("opened", opened);
    }
}
