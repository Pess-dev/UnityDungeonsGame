using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTrigger : MonoBehaviour
{
    public Interactable interactable;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<Unit>() == null)
            return;

        if (other.gameObject.GetComponentInParent<Enemy>() != null)
            return;

        interactable.Interact();
        Destroy(gameObject);
    }
}
