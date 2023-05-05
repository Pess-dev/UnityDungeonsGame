using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public float damage = 1f;

    public float knockback = 1f;

    public float damageVelocity= 5f;

    public float cooldown = 1f;

    protected float timer = 0f;

    public float bounce = 0.1f;

    [SerializeField]
    private bool canDamageByPhysics = true;

    [SerializeField]
    private bool grabbable = false;

    [SerializeField]
    private bool draggable = false;

    private bool grabbed = false;

    private InteractableItem interactable;

    private Rigidbody rb;

    private List<Health> damaged;

    public int idleAnim = 1;

    private string oldTag;

    private void Awake()
    {
        if (GetComponent<InteractableItem>() != null)
            interactable = GetComponent<InteractableItem>();

        if (GetComponent<Rigidbody>() != null)
            rb = GetComponent<Rigidbody>();

        oldTag = tag;
    }
    private void Update()
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
        if (interactable.grab != null)
            return interactable.grab; 
        else 
            return transform;
    }

    public void Grab(string newTag) 
    {
        if (!grabbable)
            return;

        gameObject.tag = newTag;

        rb.isKinematic = true;

        interactable.Deactivate();
        grabbed = true;
    }

    public void Release()
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

    public virtual bool Use() 
    {
        if (timer <= 0)
        {
            timer = cooldown;
            return true;
        }
        else
        {
            return false;
        }
    }
}
