using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //transform to set camera to
    public Transform cameraPlace;

    public Transform Head;

    //
    public Transform feetTransform;
    public float floorCheckRadius;
    //

    private bool controled = false;

    [SerializeField]
    private LayerMask lm;

    [SerializeField]
    private float maxHP;
    private float HP;
     
    private float dashTimer = 0f;
    public float dashForce = 1f;
    public float dashCooldown = 1f;

    private bool isGrounded;
    private Vector3 normalSurface;

    private float jumpTimer = 0f;
    public float jumpCooldown = 1f;
    public float jumpForce = 3f;

    //Movement
    private Vector3 velocity;
    public float speed = 5f; 

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

        Debug.Log("check");
        if (Physics.Raycast(feetTransform.position,  -1f * transform.up, out hit, floorCheckRadius, lm))
        {
            Debug.Log("contact");
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
        rb.AddForce(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
    }
    public void Rotate(Vector3 Euler)
    {
        transform.Rotate(Euler);
    }

    public void Jump() 
    {
        if (isGrounded && jumpTimer<=0)
        {  
            rb.AddForce(Vector3.up * jumpForce);
            jumpTimer = jumpCooldown;

        }
        Debug.Log(isGrounded);
        Debug.Log(jumpTimer);
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
    }
}