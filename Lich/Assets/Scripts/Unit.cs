using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //transform to set camera to
    public Transform cameraPlace;

    //head to disable when player controls unit
    public Transform Head;

    //control unit
    private bool controled = false;

    //Dash
    private float dashTimer = 0f;
    public float dashForce = 1f;
    public float dashCooldown = 1f;

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
    public float speedForce = 5f;
    public float airSpeedModifier = 0.1f;
    public float rbDrag = 1f;
    public float rbAirDrag = 0f;
    public float maxSpeed = 5f; 
    public float maxFloorAngle = 30;

    //items
    [SerializeField]
    private Transform hand;
    [SerializeField]
    private Transform back;
    [SerializeField]
    private Item firstItem;
    [SerializeField]
    private Item secondItem;

    //fight system
    [SerializeField]
    private Transform attackPoint;
    public float attackRadius; 

    //animation system
    private Animator anim;
    public float walkingAnimationVelocity = 0.5f;

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
            firstItem.Grab(tag);

        if (secondItem != null)
            secondItem.Grab(tag);
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
            moveDirection = Quaternion.FromToRotation(Vector3.up, normalFloor) * moveDirection;
        else
            moveDirection *= airSpeedModifier;

        float currentSpeed = rb.velocity.magnitude;
        float speedLimitModifier = 1f;

        if (currentSpeed >= maxSpeed)
        {
            speedLimitModifier = maxSpeed / currentSpeed;
        }

        if (isGrounded)
        {
            rb.drag = rbDrag;
        }
        else
        {
            rb.drag = rbAirDrag;
        }

        float k = 1;
        if (normalAllSurfaces.magnitude > 0 && !isGrounded)
        {
            k = Vector3.Dot(moveDirection.normalized, normalAllSurfaces.normalized);
        }

        rb.AddForce(moveDirection * speedForce * speedLimitModifier * Time.deltaTime * k, ForceMode.VelocityChange);
        
    }
    
    public void Rotate(Vector3 Euler)
    {
        transform.Rotate(Euler);
    }

    private void UpdateAnimation() 
    {
        if (anim != null)
        {
            if (rb.velocity.magnitude > walkingAnimationVelocity)
                anim.SetBool("walking", true);
            else
                anim.SetBool("walking", false);

            anim.SetBool("grounded", isGrounded);

            anim.SetBool("grabbed", CheckGrabbed()); 
        }
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
        if (dashTimer > 0 )
            return;

        moveDirection = transform.TransformDirection(moveDirection);
          
        moveDirection = Quaternion.FromToRotation(Vector3.up, normalFloor) * moveDirection;
        
        rb.AddForce(moveDirection * airSpeedModifier * dashForce, ForceMode.Impulse);

        dashTimer = dashCooldown;  
    }

    public void UseItem()
    { 
        if (firstItem == null) 
            return;

        bool result = firstItem.Use();
        if (!result)
            return; 
        anim.SetTrigger("attack");
        anim.SetInteger("grabbedUse", Random.Range(1, 4));
        MeleeAttack(firstItem.damage, firstItem.knockback);  
    }

    public void MeleeAttack(float damage, float knockback)
    {
        Collider[] hits = Physics.OverlapSphere(attackPoint.position,attackRadius);
        foreach ( Collider collider in hits)
        {
            if (collider.gameObject.tag == tag) 
                continue;
            Health targetHealth = collider.transform.GetComponent<Health>();
            if (targetHealth == null)
                continue;

            targetHealth.Damage(damage);

            Rigidbody targetRb = collider.transform.GetComponent<Rigidbody>();

            if (targetRb == null)
                continue;

            Vector3 knockbackDirection = (collider.transform.position - transform.position).normalized;

            targetRb.AddForce( knockbackDirection * knockback, ForceMode.Force);
        }
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
        item.Grab(tag);
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
        Item temp = firstItem;
        firstItem = secondItem;
        secondItem = temp;
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
        Quaternion rotationOffset = Quaternion.Inverse(obj.rotation * by.localRotation) * to.rotation;
        obj.rotation = obj.rotation * rotationOffset;

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