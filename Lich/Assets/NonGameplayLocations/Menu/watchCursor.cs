using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class watchCursor : MonoBehaviour
{

    public Camera cam;
    public float lerp = 0.1f;

    void Update()
    {
        if (cam == null)
            return;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);//Mouse.current.position.ReadValue());
        
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit))
            return;
        Vector3 target = hit.point;
        Vector3 lookForward = (target - transform.position).normalized;
        lookForward = Vector3.Lerp(transform.forward, lookForward, lerp * Time.deltaTime).normalized;
        transform.rotation = Quaternion.LookRotation(lookForward,Vector3.up);
    }
}
