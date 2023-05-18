using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{ 
    public float cooldown = 1f;
     
    protected float timer = 0f;

    public float breaking = 0f;

    [SerializeField]
    protected bool grabbable = false;

    [SerializeField]
    protected Transform grab;

    protected bool grabbed = false;

    protected Interactable interactable;

    protected Rigidbody rb;

    protected Unit user;

    public int idleAnimationNumber = 1;

    public float animationModifier = 1;

    public ItemType itemType = ItemType.NonWeapon;

    private string oldTag;

    protected Health health;


    public enum ItemType
    {
        NonWeapon,
        Melee,
        Cast,
    }

    public UnityEvent grabbedEvent;
    public UnityEvent usedEvent;

    public Unit GetUser()
    {
        return user;
    }

    protected virtual void Awake()
    {
        interactable = GetComponent<Interactable>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();

        oldTag = gameObject.tag;
    }
    protected virtual void Update()
    {
        if (timer > 0 && grabbed)
            timer -= Time.deltaTime;
    }

    public bool GetGrabbable()
    { return grabbable; }

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

    public virtual void Grab(Unit unit) 
    {
        if (!grabbable)
            return;

        user = unit;

        gameObject.tag = user.tag;

        rb.isKinematic = true;

        interactable.Deactivate();
        grabbed = true;
        grabbedEvent.Invoke();
    }

    public virtual void Release()
    {
        if (!grabbable)
            return;

        rb.isKinematic = false;

        user = null;

        gameObject.tag = oldTag;

        interactable.Activate();
        grabbed = false;
    }

    public virtual bool GetUsable()
    { return timer <= 0 ? true : false; }

    public virtual void Use()
    {
        usedEvent.Invoke();
    }

    public virtual void StopUse()
    { }
}
