using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{ 
    public float cooldown = 1f;
     
    protected float timer = 0f;

    [SerializeField]
    protected bool grabbable = false;

    [SerializeField]
    protected Transform grab;

    protected bool grabbed = false;

    protected Interactable interactable;

    protected Rigidbody rb;

    protected Unit user;

    public int idleAnimationNumber = 1;

    private string defaultTag;

    public ItemType itemType = ItemType.NonWeapon;

    public enum ItemType
    {
        NonWeapon,
        Melee,
        Cast,
    }

    public Unit GetUser()
    {
        return user;
    }

    protected virtual void Awake()
    {
        if (GetComponent<Interactable>() != null)
            interactable = GetComponent<Interactable>();

        if (GetComponent<Rigidbody>() != null)
            rb = GetComponent<Rigidbody>();

        defaultTag = gameObject.tag;
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

        rb.isKinematic = true;

        gameObject.tag = user.gameObject.tag;

        interactable.Deactivate();
        grabbed = true;
    }

    public virtual void Release()
    {
        if (!grabbable)
            return;

        rb.isKinematic = false;

        gameObject.tag = defaultTag;

        user = null;

        interactable.Activate();
        grabbed = false;
    }

    public virtual bool GetUsable()
    { return timer <= 0 ? true : false; }

    public virtual void Use()
    { }

    public virtual void StopUse()
    { }
}
