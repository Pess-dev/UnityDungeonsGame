using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //transform to set camera to
    public Transform cameraPlace;

    //head to disable when player controls unit
    public Transform Head;

    //to rotate head and some other things
    private float xRotation = 0f;

    //control unit
    private bool controled = false;

    //Dash
    private float dashTimer = 0f;
    public float dashForce = 1f;
    public float dashCooldown = 1f;
    private bool dashing = false;
    public float dashDuration = 0.2f;
    [SerializeField]
    private Object dashParticle;
    private Vector3 dashVelocity = Vector3.zero;
     

    //GroundCheck
    private bool isGrounded;
    private Vector3 normalAllSurfaces;
    private Vector3 normalFloor;
    [SerializeField]
    private LayerMask groundLayer;

    //Jump
    private float jumpTimer = 0f;
    public float jumpCooldown = 1f;
    public float jumpForce = 3f;

    //Movement
    public float speed = 1f;
    public float modifier = 0.1f;
    public float friction = 0.1f;
    public float airSpeedModifier = 0.1f;
    public float maxFloorAngle = 30;

    //items
    [SerializeField]
    private Transform hand;
    [SerializeField]
    private Transform back;
    public Item firstItem;
    public Item secondItem;

    //drop place transform
    [SerializeField]
    private Transform drop;

    //fight system 
    public Transform attackPoint;
    public float attackRadius; 

    //animation system
    private Animator anim;
    public float walkingAnimationVelocity = 0.5f;

    //interaction system
    [SerializeField]
    public float interactDistance = 3f;

    private Health health;

    private Rigidbody rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();

        health.hit.AddListener(Damaged);

        if (controled)
            SetControl();
        else
            ReleaseControl();

        if (firstItem != null)
            firstItem.Grab(this);

        if (secondItem != null)
            secondItem.Grab(this);
    }

    void Update()
    {
        if (dashTimer > 0)
            dashTimer -= Time.deltaTime;
        if (dashTimer < dashCooldown - dashDuration)
            dashing = false;
        else
            dashing = true;

        if (jumpTimer > 0)
            jumpTimer -= Time.deltaTime; 

        checkGround();
        UpdateAnimation(); 
    }
    private void LateUpdate()
    {
        MoveItems();
        RotateHead();
    }

    private void RotateHead()
    { 
        Head.rotation = Quaternion.Euler(Head.rotation.eulerAngles + Vector3.right * (xRotation - Head.rotation.eulerAngles.x));
    }

    private void checkGround()
    {
        bool lastGrounded = isGrounded;
        if (normalFloor.magnitude > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (lastGrounded != isGrounded)
            jumpTimer = jumpCooldown; 
    }

    public void Move(Vector3 moveDirection)
    {
        moveDirection = transform.TransformDirection(moveDirection);
        if (isGrounded)
        { 
            moveDirection = Quaternion.FromToRotation(Vector3.up, normalFloor) * moveDirection;
        }
        else
        { 
            moveDirection *= airSpeedModifier;
        }

        if (rb.velocity.magnitude > speed && isGrounded)
        {
            moveDirection *= speed / rb.velocity.magnitude;
            moveDirection -= Vector3.Project(moveDirection, rb.velocity);
        }

        Vector3 addVelocity = moveDirection * speed;
        Vector3 flatVelocity = rb.velocity - rb.velocity.y * Vector3.up;
        Vector3 newFlatVelocity = addVelocity + flatVelocity;
        if (newFlatVelocity.magnitude > speed)
        {
            addVelocity = newFlatVelocity * speed / newFlatVelocity.magnitude - flatVelocity; 
        }

        if (dashing)
            rb.velocity = Vector3.Lerp(rb.velocity, dashVelocity, 0.7f);
        else
            rb.velocity += addVelocity * modifier;

        if (isGrounded && moveDirection.magnitude == 0)
        {
            rb.velocity += -flatVelocity * friction;
        }
    }

    public void Rotate(Vector3 Euler)
    {
        transform.Rotate(Euler);
    }
    public void setXRotation(float rot)
    {
        xRotation = rot;
    }


    private void UpdateAnimation() 
    {
        if (rb.velocity.magnitude > walkingAnimationVelocity)
            anim.SetBool("walking", true);
        else
            anim.SetBool("walking", false);

        anim.SetBool("grounded", isGrounded);


        if (firstItem != null)
        {
            anim.SetInteger("grabbedIdle", firstItem.idleAnimationNumber);
            anim.SetBool("grabbed", true);
        }
        else
            anim.SetBool("grabbed", false);
        anim.SetBool("dashing", dashing);
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
        if (dashTimer > 0 || moveDirection.magnitude == 0)
            return;

        moveDirection = transform.TransformDirection(moveDirection);
        
       if (isGrounded) 
            moveDirection = Quaternion.FromToRotation(Vector3.up, normalFloor) * moveDirection;

        dashVelocity = moveDirection.normalized * dashForce;
        dashTimer = dashCooldown;

        Instantiate(dashParticle, transform.position, Quaternion.LookRotation(-moveDirection.normalized), this.transform);
    }

    public void UseItem()
    { 
        if (firstItem == null) 
            return;

        if (firstItem.GetTimer()>0)
            return;

        firstItem.Use(); 

        anim.SetTrigger("attack");
        anim.SetInteger("grabbedUse", Random.Range(1, 4));
    }

    private void Damaged()
    { 
        anim.SetTrigger("damaged");
        anim.SetInteger("damage", Random.Range(1,4));
    }

    public void GrabItem(Item item)
    {
        if (!item.GetGrabbable())
            return;
        DiscardItem();

        firstItem = item;
        item.Grab(this);
    }

    public void DiscardItem()
    {
        if (firstItem == null)
            return;

        firstItem.Release();
        firstItem = null;
    }

    public void SwitchItems()
    {
        if (firstItem == null && secondItem == null)
            return;

        if (firstItem != null) firstItem.StopUse();
        if (secondItem != null) secondItem.StopUse();

        Item temp = firstItem;
        firstItem = secondItem;
        secondItem = temp;
        anim.SetTrigger("switch");
    }

    public bool CheckGrabbed()
    {
        if (firstItem != null)
            return true;
        else
            return false;
    }

    private void MoveItems()
    {
        if (firstItem != null)
        {
            MoveBy(firstItem.transform, firstItem.GetGrabTransform(), hand);
        }
        if (secondItem != null)
        {
            MoveBy(secondItem.transform, secondItem.GetGrabTransform(), back);
        }
        
    }
    private void MoveBy(Transform obj, Transform by, Transform to)
    {
        //Quaternion rotationOffset = Quaternion.Inverse(obj.rotation * by.localRotation) * to.rotation;
        //obj.rotation = obj.rotation * rotationOffset;
        obj.rotation = to.rotation * by.localRotation;


        Vector3 positionOffset = to.position - by.position;
        obj.position = obj.position + positionOffset;
    }

    public void SetControl()
    {
        controled = true;

        if (Head != null)
            ToggleHeadVisible(false);
    }
    public void ReleaseControl()
    {
        controled = false;

        if (Head != null)
            ToggleHeadVisible(true);
    }

    public void ToggleHeadVisible(bool visible = true)
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
         
        normalAllSurfaces = Vector3.zero;
        normalFloor = Vector3.zero;

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.contacts[i].normal;
            float angle = Vector3.Angle(Vector3.up, normal);

            if (angle <= maxFloorAngle)
            {
                normalFloor += normal;
            }
            normalAllSurfaces += normal; 
        }
        normalAllSurfaces.Normalize();
        normalFloor.Normalize();
    }
    private void OnCollisionExit(Collision collision)
    {
        int layer = collision.gameObject.layer;

        if (groundLayer != (groundLayer | (1 << layer))) return;

        normalFloor = Vector3.zero;
        normalAllSurfaces = Vector3.zero;
    }


    private void OnDrawGizmosSelected()
    {         
        Gizmos.color = Color.red; 
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + normalFloor); 
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + normalAllSurfaces); 
    }

    private void OnDestroy()
    {
        if (firstItem != null)
            firstItem.Release();
        if (secondItem != null)
            secondItem.Release();
    }
}