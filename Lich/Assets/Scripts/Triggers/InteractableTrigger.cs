using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTrigger : MonoBehaviour
{
    public Interactable interactable;
    private void OnTriggerEnter(Collider other)
    {
        interactable.Interact();
        Destroy(gameObject);
    }
}
