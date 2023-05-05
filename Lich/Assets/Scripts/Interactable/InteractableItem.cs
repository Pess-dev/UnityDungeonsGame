using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : Interactable
{
    private Item item;

    public Transform grab;

    private void Start()
    {
        if(GetComponent<Item>() != null)
            item = GetComponent<Item>();

        canGrab = item.GetGrabbable();
        canDrag = item.GetDraggable();
    }
}
