using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage;

    [SerializeField]
    protected bool canGrab;

    [SerializeField]
    protected bool active = false;

    public void BaseInteract()
    {
        Interact();
    }

    public bool GetCanGrab()
    {
        return canGrab;
    }

    public bool getActive()
    {
        return active;
    }

    protected virtual void Interact() { }
    public virtual void Activate() { }
    public virtual void Deactivate() { }
}
