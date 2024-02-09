using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour
{
    //transform to set camera to
    public Transform cameraPlace;

    //head to disable when player controls unit
    public Transform Head;

    //to rotate head and some other things
    public float xRotation = 0f;

    //control unit
    private bool controled = false;

    //Dash
    private float dashTimer = 0f;
    public float dashVelocityMagnitude = 1f;
    public float dashCooldown = 1f;
    private bool dashing;
    public float dashDuration = 0.2f;
    public float dashLerp = 0.7f;
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
    public float maxSpeed = 1f;
    public float movementForce = 0.1f;
    public float airMovementForce = 0.1f;
    public float maxFloorAngle = 30;
    public Vector3 moveDirection {get; private set;}

    //items
    [SerializeField]
    private Transform hand;
    [SerializeField]
    private Transform back;
    public Item firstItem;
    public Item secondItem;

    //drop spawn place transform
    [SerializeField]
    private Transform drop;

    //fight system 
    public Transform attackPoint;

    //animation system
    private Animator anim;
    public float walkingAnimationVelocity = 0.5f;

    //interaction system
    [SerializeField]
    public float interactDistance = 3f;

    public Health health;

    private Rigidbody rb;

    public Team side = Team.Neutral;

    public UnityEvent attackEvent;
    public UnityEvent dashEvent;

    private float lifeTime = 0;

    public enum Team
    {
        Neutral,
        Skeleton,
        Goblin,
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();

        health.hit.AddListener(Damaged);
        health.death.AddListener(OnDeath);
        if (cameraPlace == null)
            cameraPlace = Head;
    }

    void Start()
    {
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
        lifeTime += Time.deltaTime;

        if (dashTimer > 0) 
            if (isGrounded || dashTimer > dashCooldown - dashDuration)
                dashTimer -= Time.deltaTime;
            else 
                dashTimer -= Time.deltaTime*0.5f;

        if (dashTimer < dashCooldown - dashDuration)
        {
            if (dashing && rb.velocity.magnitude > 0)
                rb.velocity = (rb.velocity - rb.velocity.y * Vector3.up) * maxSpeed / rb.velocity.magnitude + rb.velocity.y * Vector3.up;
            dashing = false;
            health.mortal = true;
        }
        else
        {
            health.mortal = false;
            dashing = true;
            if (!isGrounded) 
                dashVelocity -= Vector3.up*dashVelocity.y;
        }

        if (jumpTimer > 0)
            jumpTimer -= Time.deltaTime; 

        UpdateAnimation(); 
    }

    private void LateUpdate()
    {
        MoveItems();
        RotateHead();
    }

    private void FixedUpdate()
    {
        Move();
        checkGround();
    }

    private void RotateHead()
    { 
        Head.rotation = Quaternion.Euler(Head.rotation.eulerAngles + Vector3.right * (-xRotation - Head.rotation.eulerAngles.x));
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

    public void SetMoveDirection(Vector3 moveDirection)
    {
        this.moveDirection = moveDirection;
    }
    
    public void Move()
    {
        Vector3 direction = Vector3.ProjectOnPlane(moveDirection, transform.up);

        Vector3 planeVelocity = Vector3.ProjectOnPlane(rb.velocity, transform.up);

        Vector3 addVelocity = direction * movementForce / rb.mass * Time.fixedDeltaTime;

        if ((planeVelocity + addVelocity).magnitude > maxSpeed)
        {
            addVelocity = (planeVelocity + addVelocity).normalized * maxSpeed - planeVelocity;
        }

        if (addVelocity.magnitude == 0)
        {
            addVelocity = -planeVelocity.normalized * movementForce / rb.mass * Time.fixedDeltaTime;
            if (Vector3.Dot(addVelocity + planeVelocity, planeVelocity) < 0) addVelocity = -planeVelocity;
        }

        if (dashing)
            rb.velocity = Vector3.Lerp(rb.velocity, dashVelocity, dashLerp * Time.deltaTime);
        else
            rb.velocity += addVelocity;           
    }

    public void RotateLocal(Vector3 deltaEuler)
    {
        transform.Rotate(Vector3.up * deltaEuler.y);
        xRotation += deltaEuler.x;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);
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
            anim.SetFloat("attackModifier", firstItem.animationModifier);
        }
        else
            anim.SetBool("grabbed", false);

        anim.SetBool("dashing", dashing);
    }
    
    public void Jump() 
    {
        if (isGrounded && jumpTimer<=0)
        {  
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
            jumpTimer = jumpCooldown; 
        } 
    }

    public void Dash(Vector3 moveDirection)
    {
        if (dashTimer > 0 || moveDirection.magnitude == 0)
            return;
        //// Quaternion.FromToRotation(Vector3.up, normalFloor) * newVelocity;
        //if (isGrounded)
        //    moveDirection = Quaternion.FromToRotation(Vector3.up, normalFloor) * moveDirection;

        dashVelocity = moveDirection.normalized * dashVelocityMagnitude;
        dashTimer = dashCooldown;

        Instantiate(dashParticle, transform.position, Quaternion.LookRotation(-moveDirection.normalized), this.transform);
        dashEvent.Invoke();
    }

    public void UseItem()
    { 
        if (firstItem == null) 
            return;

        if (firstItem.GetTimer()>0)
            return;

        firstItem.Use();

        attackEvent.Invoke();

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
        firstItem.gameObject.GetComponentInChildren<MeshRenderer>().gameObject.layer = 8;
        item.Grab(this);
    }

    public void DiscardItem()
    {
        if (firstItem == null)
            return;
        firstItem.gameObject.GetComponentInChildren<MeshRenderer>().gameObject.layer = 7;
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
            int childCount = q.Peek().childCount;

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

    public bool GetDashing() { return dashing; }


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
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + normalFloor); 
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + normalAllSurfaces); 
    }

    private void OnDeath()
    {
        if (firstItem != null)
            firstItem.GetComponent<Health>().Kill();
        if (secondItem != null)
            secondItem.GetComponent<Health>().Kill();
    }

    private void OnDestroy()
    {
        if (firstItem != null)
            Destroy(firstItem.gameObject);
        if (secondItem != null)
            Destroy(secondItem.gameObject);
    }

    public float GetDashTimer()
    {
        return dashTimer;
    }

    public float GetLifeTime()
    {
        return lifeTime;
    }
}