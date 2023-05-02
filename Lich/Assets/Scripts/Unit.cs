using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //transform to set camera to
    public Transform cameraPlace;

    //head to disable when player controls unit
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

    //Jump
    private float jumpTimer = 0f;
    public float jumpCooldown = 1f;
    public float jumpForce = 3f;

    //Movement 
    public float speedForce = 5f;
    public float airSpeedModifier = 0.1f;
    public float maxSpeed = 5f; 
    public float maxFloorAngle = 30;

    //weapon and items
    [SerializeField]
    private Transform hand;
    [SerializeField]
    private Transform back;
    [SerializeField]
    private Interactable firstItem;
    [SerializeField]
    private Interactable secondItem;

    //animation system
    private Animator anim;
    public float walkingAnimationSpeed = 0.5f;

    private Rigidbody rb;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
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
        UpdateAnimation();
    }
    private void LateUpdate()
    {
        MoveItems();
    }

    private void checkGround()
    {
        bool lastGrounded = isGrounded;

        if (normalSurface.magnitude > 0)
            isGrounded = true;
        else
            isGrounded = false;

        if (lastGrounded != isGrounded)
            jumpTimer = jumpCooldown; 
    }

    public void Move(Vector3 moveDirection)
    {
        moveDirection = transform.TransformDirection(moveDirection);
        if (isGrounded)
            moveDirection = Quaternion.FromToRotation(Vector3.up, normalSurface) * moveDirection;
        else
            moveDirection *= airSpeedModifier;

        float currentSpeed = rb.velocity.magnitude;
        float speedLimitModifier = 1f;

        if (currentSpeed >= maxSpeed)
        {
            speedLimitModifier = maxSpeed / currentSpeed;
        }

        rb.AddForce(moveDirection * speedForce * speedLimitModifier * Time.deltaTime, ForceMode.VelocityChange);
    }

    private void UpdateAnimation() 
    {
        if (anim != null)
        {
            if (rb.velocity.magnitude > walkingAnimationSpeed)
                anim.SetBool("walking", true);
            else
                anim.SetBool("walking", false);

            if (firstItem != null)
                anim.SetBool("melee", true);
            else
                anim.SetBool("melee", false);

        }
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

    public void Dash(Vector3 moveDirection)
    {
        if (dashTimer > 0)
            return;

        moveDirection = transform.TransformDirection(moveDirection);

        if (isGrounded)            
            moveDirection = Quaternion.FromToRotation(Vector3.up, normalSurface) * moveDirection;
        
        rb.AddForce(moveDirection * dashForce, ForceMode.Impulse);

        dashTimer = dashCooldown;  
    }

    public void GrabInteractable(Interactable item)
    {
        ClearItem();
        if (item.GetCanGrab())
        {
            firstItem = item;
            firstItem.Deactivate();
        }
    }

    public void ClearItem()
    {
        if (firstItem != null)
        {
            firstItem.Activate();
            firstItem = null;
        }
    }

    public void SwitchItems()
    {
        Interactable temp = firstItem;
        firstItem = secondItem;
        secondItem = temp;
    }

    private void MoveItems()
    {
        if (firstItem != null)
        {
            firstItem.transform.position = hand.position;
            firstItem.transform.rotation = hand.rotation;
        }
        if (secondItem != null)
        {
            secondItem.transform.position = back.position;
            secondItem.transform.rotation = back.rotation;
        }
    }

    public void setControl()
    {
        controled = true;

        if (Head != null)
            toggleHeadVisible(false);
    }
    public void releaseControl()
    {
        controled = false;

        if (Head != null)
            toggleHeadVisible(true);
    }

    public void toggleHeadVisible(bool visible = true)
    {
        UnityEngine.Rendering.ShadowCastingMode mode;
        if (visible)
            mode = UnityEngine.Rendering.ShadowCastingMode.On;
        else
            mode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

        Queue<Transform> q = new Queue<Transform>();
        q.Enqueue(Head);
        while (q.Count > 0)
        {
            int childCount = q.Peek().childCount; // получаем количество детей у объекта parent

            for (int i = 0; i < childCount; i++)
            {
                Transform child = q.Peek().transform.GetChild(i);
                q.Enqueue(child);
            }
            if (q.Peek().GetComponent<MeshRenderer>() != null)
                q.Peek().GetComponent<MeshRenderer>().shadowCastingMode = mode;
            q.Dequeue();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        int layer = collision.gameObject.layer;

        if (groundLayer != (groundLayer | (1 << layer))) return;

        Vector3 resultNormal = Vector3.zero;

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.contacts[i].normal;
            
            //is floor?
            if (Vector3.Angle(Vector3.up,normal) <= maxFloorAngle)
            {
                resultNormal += normal;
            }
        }

        if (resultNormal.magnitude != 0)
            normalSurface = resultNormal.normalized;
        else
            normalSurface = resultNormal;
    }
    private void OnCollisionExit(Collision collision)
    {
        int layer = collision.gameObject.layer;

        if (groundLayer != (groundLayer | (1 << layer))) return;

        normalSurface = Vector3.zero;

    }


    private void OnDrawGizmosSelected()
    {         
        Gizmos.color = Color.red; 
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.FromToRotation(Vector3.up, normalSurface) * transform.forward);
        
    }
}