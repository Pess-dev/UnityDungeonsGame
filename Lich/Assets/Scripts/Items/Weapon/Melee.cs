using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Item
{
    public float knockback = 1f;

    private float attackDurationTimer = 0;

    public float attackDuration = 0.1f;

    Unit user;

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
        user = null;
        attackDurationTimer = 0f;
    }
    public override void Use(Unit unit)
    {
        if (timer > 0)
            return;

        attackDurationTimer = attackDuration;
        user = unit;
        timer = cooldown;
        damaged.Clear();
    }

    public void MeleeAttack(Unit unit)
    {
        Collider[] hits = Physics.OverlapSphere(unit.attackPoint.position, unit.attackRadius);
        foreach (Collider collider in hits)
        {
            if (collider.gameObject.tag == unit.tag)
                continue;

            Health targetHealth = collider.transform.GetComponent<Health>();

            if (targetHealth == null)
                continue;
            if (damaged.Contains(targetHealth))
                continue;

            targetHealth.Damage(damage);

            Rigidbody targetRb = collider.transform.GetComponent<Rigidbody>();

            if (targetRb == null)
                continue;

            Vector3 knockbackDirection = (collider.transform.position - transform.position).normalized;

            targetRb.AddForce(knockbackDirection * knockback, ForceMode.Force);

            damaged.Add(targetHealth);
        }
    }

}
