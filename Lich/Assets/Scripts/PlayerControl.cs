using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Unit unit;

    //Look
    public Camera cam; 
    private float xRotation = 0f;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    private void Start()
    {
        if (unit != null)
        {
            unit.setControl();
        }
    }

    private void Update()
    {
        SyncWithUnit();
    }

    public void SyncWithUnit() 
    {
        if (unit == null)
            return;

        Vector3 eye;

        if (unit.cameraPlace == null)
            eye = unit.transform.position;
        else
            eye = unit.cameraPlace.position;
        cam.transform.position = eye;
        cam.transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles+Vector3.up*( unit.transform.rotation.eulerAngles.y - cam.transform.rotation.eulerAngles.y));
    }

    public void ProcessLook(Vector2 input)
    {
        if (unit == null)
            return; 

        xRotation -= (input.y * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        unit.Rotate(Vector3.up * (input.x * Time.deltaTime) * xSensitivity);

    }

    public void ProcessMove(Vector2 input) 
    {
        if (unit == null)
            return;

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        unit.Move(moveDirection);
    }

    public void Dash() 
    {
        if (unit == null)
            return;

        unit.Dash();
    }

    public void Jump()
    {
        if (unit == null)
            return;
        unit.Jump(); 
    }
}
