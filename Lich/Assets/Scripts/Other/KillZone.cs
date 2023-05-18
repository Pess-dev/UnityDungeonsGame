using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Health health = other.gameObject.GetComponentInParent<Health>();
        if(health != null)
            health.Kill();
    }
}
