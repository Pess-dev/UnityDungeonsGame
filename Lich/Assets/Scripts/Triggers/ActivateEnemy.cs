using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateEnemy : MonoBehaviour
{
    [SerializeField]
    private Vector3 halfExtends;
    [SerializeField]
    private Transform position;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Unit>() == null)
            return;

        if (other.GetComponentInParent<Enemy>() != null)
            return;

        Collider[] colliders = Physics.OverlapBox(position.position, halfExtends, position.rotation);
        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy == null)
                continue;
            enemy.active = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (position!=null)
            Gizmos.DrawWireCube(position.position, position.rotation * halfExtends);
    }
}
