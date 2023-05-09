using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Unit unit;

    private void Start()
    {
        unit = GetComponent<Unit>();
    }

    private void Update()
    {
        if (unit.firstItem == null)
            GrabNearestItem();
    }

    private void GrabNearestItem()
    {
        Item item = null;

        Collider[] collisions = Physics.OverlapSphere(unit.cameraPlace.position, unit.interactDistance);

        foreach (Collider collision in collisions)
        {
            if (collision.transform.GetComponent<Item>() == null)
                continue;

            Item collisionItem = collision.transform.GetComponent<Item>();

            if (collisionItem.GetGrabbed())
                continue;
               
            if (item == null)
            {
                item = collisionItem;
                continue;
            }

            if ((collisionItem.transform.position - transform.position).magnitude < (item.transform.position - transform.position).magnitude)
                item = collisionItem; 
        }

        if (item != null)
            unit.GrabItem(item);
    }
}
