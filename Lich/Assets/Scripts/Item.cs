using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float damage = 1f;

    public float damageVelocity= 5f;

    public float cooldown = 1f;

    protected float timer = 0f;

    public float bounce = 0.1f;

    [SerializeField]
    protected bool canDamageByPhysics = true;

    [SerializeField]
    protected bool grabbable = false;

    [SerializeField]
    protected bool draggable = false;

    [SerializeField]
    protected Transform grab;

    protected bool grabbed = false;

    protected Interactable interactable;

    protected Rigidbody rb;

    protected List<Health> damaged = new List<Health>();

    protected string oldTag;

    private void Awake()
    {
        if (GetComponent<Interactable>() != null)
            interactable = GetComponent<Interactable>();

        if (GetComponent<Rigidbody>() != null)
            rb = GetComponent<Rigidbody>();

        oldTag = tag;
    }
    protected virtual void Update()
    {
        if (timer > 0 && grabbed)
            timer -= Time.deltaTime;
    }

    

    public bool GetGrabbable()
    { return grabbable; }

    public bool GetDraggable()
    { return draggable; }

    public bool GetGrabbed()
    { return grabbed; }
    public float GetTimer()
    { return timer; }

    public Transform GetGrabTransform()
    {
        if (grab != null)
            return grab; 
        else 
            return transform;
    }

    public virtual void Grab(string newTag) 
    {
        if (!grabbable)
            return;

        gameObject.tag = newTag;

        rb.isKinematic = true;

        interactable.Deactivate();
        grabbed = true;
    }

    public virtual void Release()
    {
        if (!grabbable)
            return;

        rb.isKinematic = false;

        gameObject.tag = oldTag;

        interactable.Activate();
        grabbed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canDamageByPhysics || grabbed)
            return;
        Health health = collision.transform.GetComponent<Health>();
        if (health == null)
            return;

        Vector3 resultVelocity = collision.relativeVelocity;

        Vector3 normal = transform.position - collision.collider.ClosestPoint(transform.position);

        if (resultVelocity.magnitude > damageVelocity)
        {
            damaged.Add(health);
            health.ChangeHP(-damage);
            rb.velocity = Vector3.Reflect(resultVelocity*bounce, normal);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!canDamageByPhysics || grabbed)
            return;
        Health health = collision.transform.GetComponent<Health>();
        if (health == null)
            return;
        if (damaged.Contains(health)) 
            damaged.Remove(health);
    }

    public virtual void Use(Unit unit)
    {}
}
