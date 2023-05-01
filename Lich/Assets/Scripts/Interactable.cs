using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage;

    [SerializeField]
    protected bool canGrab;

    public void BaseInteract()
    {
        Interact();
    }



    public bool GetCanGrab()
    {
        return canGrab;
    }

    protected virtual void Interact(){}
}
