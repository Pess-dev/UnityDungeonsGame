using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorsCheck : MonoBehaviour
{
    [SerializeField]
    private Vector3 halfExtends;

    [SerializeField]
    private Transform position;

    [SerializeField]
    private Door door;

    private void Start()
    {
        door.interacted.AddListener(Check);
        door.viewed.AddListener(Check);
    }

    private void Check()
    {
        if (position == null)
        {
            door.SetLocked(false);
            return;
        }

        Collider[] colliders = Physics.OverlapBox(position.position, halfExtends, position.rotation);
        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                door.SetLocked(true);
                return;
            }
        }
        door.SetLocked(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (position != null)
            Gizmos.DrawWireCube(position.position, position.rotation * halfExtends);
    }
}
