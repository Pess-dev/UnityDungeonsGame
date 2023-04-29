using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private CharacterController controller;

    public Unit unit;


    //Movement
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    //Look
    public Camera cam;
    private float xRotation = 0f;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;


    void Start()
    {
        controller = unit.gameObject.GetComponent<CharacterController>();    
    }

    void FixedUpdate()
    {
        isGrounded = controller.isGrounded;
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

        if (unit.Eye == null)
            eye = unit.transform.position;
        else
            eye = unit.Eye.position;
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
        unit.transform.Rotate(Vector3.up * (input.x * Time.deltaTime) * xSensitivity);
    }

    public void ProcessMove(Vector2 input) 
    {
        if (unit == null)
            return;

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;

        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = gravity/3;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (unit == null)
            return;

        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(-1f*jumpHeight * gravity);
        }    
    }
}
