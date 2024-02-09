using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : Interactable
{
    Item item;

    private void Start()
    {
        item = GetComponent<Item>();
    }

    public override void Interact(Unit user = null){
        base.Interact(user);
        if (user == null)
            return;
        if (item == null)
            return; 
        user.GrabItem(item);
    }
}
