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
    }
    
    private void UpdateAnimations()
    {
        if (GetComponent<Animator>() != null)
            GetComponent<Animator>().SetFloat("modifier",mana/maxMana);
    }

    public override void Use()
    {
        if (timer > 0)
            return;

        if (castObject == null)
            return;

        if (mana < manaPerCast)
            return;
         

        Projectile spawned = ((GameObject)Instantiate(castObject, castPlace.position, castPlace.rotation)).GetComponent<Projectile>();
        spawned.setDirection(user.Head.forward);
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
