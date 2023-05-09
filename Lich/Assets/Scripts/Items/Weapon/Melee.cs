using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Item
{
    public float knockback = 1f;

    private float attackDurationTimer = 0;

    public float attackDuration = 0.1f;

    public float damage = 1f;

    private List<Health> damaged = new List<Health>();

    protected override void Update()
    {
        base.Update();
        if (attackDurationTimer > 0)
        {
            attackDurationTimer -= Time.deltaTime;
            if (user != null)
                MeleeAttack(user); 
        }
    }
    public override void Release()
    {
        base.Release();
        attackDurationTimer = 0f;
    }

    public override void Use()
    {
        if (timer > 0)
            return;

        attackDurationTimer = attackDuration; 
        timer = cooldown;
        damaged.Clear();
    }

    public override void StopUse()
    {  
        attackDurationTimer = 0f;
        damaged.Clear();
    }

    public void MeleeAttack(Unit unit)
    {
        Collider[] hits = Physics.OverlapSphere(unit.attackPoint.position, unit.attackRadius);
        foreach (Collider collider in hits)
        {
            if (collider.gameObject.tag == gameObject.tag)
                continue;
             
            Health targetHealth = collider.transform.GetComponent<Health>();

            if (targetHealth == null)
                continue;
             
            if (damaged.Contains(targetHealth))
                continue; 

            targetHealth.Damage(damage);

            damaged.Add(targetHealth);
             
            Rigidbody targetRb = collider.transform.GetComponent<Rigidbody>();

            if (targetRb == null)
                continue;

            Vector3 knockbackDirection = (collider.transform.position - transform.position).normalized;

            targetRb.AddForce(knockbackDirection * knockback, ForceMode.Force);

        }
    }

}
