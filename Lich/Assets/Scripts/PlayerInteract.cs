using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    public Camera cam;
    [SerializeField]
    private float distance = 3f;

    [SerializeField]
    private LayerMask mask;

    void Update()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance, mask))
        {
            if (hit.collider.GetComponent<Interactable>() != null) 
            {
                Debug.Log(hit.collider.GetComponent<Interactable>().promptMessage);
            }
        }

    }
    private void OnDrawGizmosSelected()
    {
        if(cam != null)
            Gizmos.DrawLine( cam.transform.position, cam.transform.position +cam.transform.forward * distance);
    }
}
