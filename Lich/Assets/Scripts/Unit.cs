using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //transform to set camera to
    public Transform cameraPlace;

    public Transform Head;

    //wtf idk
    private bool controled = false;

    //heal points
    [SerializeField]
    private float maxHP;
    private float HP;

    //Dash
    private float dashTimer = 0f;
    public float dashForce = 1f;
    public float dashCooldown = 1f;

    //GroundCheck
    private bool isGrounded;
    private Vector3 normalSurface;
    [SerializeField]
    private LayerMask groundLayer;
    public Transform feetTransform;
    public float floorCheckRadius;

    //Jump
    private float jumpTimer = 0f;
    public float jumpCooldown = 1f;
    public float jumpForce = 3f;

    //Movement 
    public float speed = 5f;
    public float maxSpeed = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        HP = maxHP;

        if (controled)
            setControl();
        else
            releaseControl();
    }

    void Update()
    {
        if (dashTimer > 0)
            dashTimer -= Time.deltaTime; 
        if (jumpTimer > 0)
            jumpTimer -= Time.deltaTime;

        checkGround();
    }

    private void checkGround()
    {
        if (feetTransform == null)
        {
            isGrounded = false;
            return;
        }

        bool lastGrounded = isGrounded;

        RaycastHit hit;

        if (Physics.Raycast(feetTransform.position,  -1f * Vector3.up, out hit, floorCheckRadius, groundLayer))
        {
            normalSurface = hit.normal;
            isGrounded = true;
        }
        else
            isGrounded = false;

        if (lastGrounded != isGrounded)
            jumpTimer = jumpCooldown;
        
    }

    public void Move(Vector3 moveDirection)
    {
        moveDirection = transform.TransformDirection(moveDirection);
        if (isGrounded)
        {
            moveDirection = Quaternion.FromToRotation(Vector3.up, normalSurface) * moveDirection;
        } 

        rb.AddForce(moveDirection * speed, ForceMode.Force);

        Vector3 flatVelocity = rb.velocity - Vector3.up * rb.velocity.y;
        if (flatVelocity.magnitude > maxSpeed)
            rb.velocity = flatVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;
    }
    public void Rotate(Vector3 Euler)
    {
        transform.Rotate(Euler);
    }

    public void Jump() 
    {
        if (isGrounded && jumpTimer<=0)
        {  
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
            jumpTimer = jumpCooldown; 
        } 
    }

    public void Dash()
    {
        if (dashTimer > 0)
            return;

        Debug.Log("Dash");
        dashTimer = dashCooldown;  
    }

    public void setControl()
    {
        controled = true;

        if(Head != null)
            Head.gameObject.SetActive(false);
    }
    public void releaseControl()
    {
        controled = false;

        if (Head != null)
            Head.gameObject.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {       
        if (feetTransform!=null)
            Gizmos.DrawLine(feetTransform.position, feetTransform.position - Vector3.up*floorCheckRadius);

        if (isGrounded)
        {
            Gizmos.color = Color.red; 
            Gizmos.DrawLine(feetTransform.position, feetTransform.position + Quaternion.FromToRotation(Vector3.up, normalSurface) * transform.forward);
        }
    }
}