using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //transform to set camera to
    public Transform cameraPlace;

    //public Transform Head;

    private bool controled = false;

    [SerializeField]
    private float maxHP;
    private float HP;

    [SerializeField]
    private float dashForce;
    [SerializeField]
    private float dashCooldown;
    private float dashTimer=0;

    public float weigth=1;

    //Movement
    private Vector3 velocity;
    private bool isGrounded;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    private CharacterController controller;

    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        HP = maxHP;
    }

    void Update()
    {
        if (dashTimer > 0)
            dashTimer -= Time.deltaTime;

        isGrounded = controller.isGrounded;
    }

    public void Move(Vector3 moveDirection)
    {
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        if (isGrounded && velocity.y < 0)
            velocity.y = gravity / 3;

        //if ((velocity - Vector3.up * velocity.y).magnitude > 0)
          //  velocity -= (velocity - Vector3.up * velocity.y).normalized * weigth * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
    public void Rotate(Vector3 Euler)
    {
        transform.Rotate(Euler);
    }

    public void Jump() 
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(-1f * jumpHeight * gravity);
        }
    }

    public void Dash()
    {
        if (dashTimer > 0)
            return;

        Debug.Log("Dash");
        dashTimer = dashCooldown;

        Vector3 dashDirection = (velocity-Vector3.up*velocity.y).normalized;
        if (dashDirection.magnitude == 0)
            return;
        //  velocity += dashDirection * dashForce * Time.deltaTime;
        
    }

    public void setControl()
    {
        controled = true;
    }
    public void releaseControl()
    {
        controled = false;
    }
}
