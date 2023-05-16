using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cast : Item
{
    public float maxMana = 1;
    public float manaRecover = 0.1f;
    public float mana = 0f;
    public float manaPerCast = 0.5f;


    [SerializeField]
    private Object castObject;
    
    [SerializeField]
    private Transform castPlace;

    protected override void Awake()
    {
        base.Awake();

        itemType = ItemType.Cast;
    }

    private void Start()
    {
        mana = maxMana;
    }

    protected override void Update()
    {
        base.Update();
        mana += manaRecover * Time.deltaTime;
        if (mana > maxMana)
            mana = maxMana;

        UpdateAnimations();
    }
    
    private void UpdateAnimations()
    {
        if (GetComponent<Animator>() != null)
            GetComponent<Animator>().SetFloat("modifier",mana/maxMana);
    }

    public override void Use()
    {
        base.Use();
        if (timer > 0)
            return;

        if (castObject == null)
            return;

        if (mana < manaPerCast)
            return;
         

        Projectile spawned = ((GameObject)Instantiate(castObject, castPlace.position, castPlace.rotation)).GetComponent<Projectile>();

        Vector3 direction = user.Head.forward;

        Ray ray = new Ray(user.Head.position, Quaternion.AngleAxis(-user.xRotation, user.transform.right) * user.transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            direction = (hit.point - castPlace.position).normalized;
        }

        spawned.setDirection(direction);
        spawned.gameObject.tag = gameObject.tag;
        
        timer = cooldown;
    }

    private void OnDrawGizmosSelected()
    {
        if (user != null)
        {
            Gizmos.DrawLine(castPlace.position, castPlace.position + user.Head.forward);
        }
    }
}
