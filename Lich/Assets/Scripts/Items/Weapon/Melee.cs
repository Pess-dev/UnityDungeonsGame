using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Item
{
    public float knockback = 1f;

    private float attackDurationTimer = 0;

    public float attackDuration = 0.2f;
    public float attackDelay = 0.1f;

    public float damage = 1f;

    public float attackRadius;

    public float attackAngle;

    private List<Health> damaged = new List<Health>();


    protected override void Awake()
    {
        base.Awake();

        itemType = ItemType.Melee;

        animationModifier = 0.5f/attackDuration;
    }

    protected override void Update()
    {
        base.Update();
        if (attackDurationTimer > 0)
        {
            attackDurationTimer -= Time.deltaTime;
            if (user != null && attackDurationTimer < attackDuration - attackDelay)
                MeleeAttack(); 
        }
    }
    public override void Release()
    {
        base.Release();
        attackDurationTimer = 0f;
    }

    public override void Use()
    {
        base.Use();
        if (timer > 0)
            return;

        health.Damage(breaking);

        attackDurationTimer = attackDuration; 
        timer = cooldown;
        damaged.Clear();
    }

    public override void StopUse()
    {  
        attackDurationTimer = 0f;
        damaged.Clear();
    }

    public void MeleeAttack()
    {
        Collider[] hits = Physics.OverlapSphere(user.attackPoint.position, attackRadius);

        Vector3 endPoint = user.attackPoint.position + user.transform.forward * attackRadius;

        foreach (Collider collider in hits)
        {
            Health targetHealth = collider.transform.GetComponentInParent<Health>();



            if (targetHealth == null || targetHealth.gameObject.tag == gameObject.tag)
                continue;

            if (Vector3.Angle(user.transform.forward * attackRadius, collider.ClosestPoint(endPoint) - user.attackPoint.position) > attackAngle)
                continue;
             
            if (damaged.Contains(targetHealth))
                continue; 

            targetHealth.Damage(damage);

            damaged.Add(targetHealth);
             
            Rigidbody targetRb = collider.transform.GetComponentInParent<Rigidbody>();

            if (targetRb == null)
                continue;

            Vector3 knockbackDirection = (collider.transform.position - transform.position).normalized;

            targetRb.AddForce(knockbackDirection * knockback, ForceMode.Force);

        }
    }

    private void OnDrawGizmos()
    {
        if (user == null)
            return;

        Vector3 dir = user.transform.forward * attackRadius;

        Gizmos.DrawLine(user.attackPoint.position, user.attackPoint.position + dir);
        Gizmos.DrawLine(user.attackPoint.position, user.attackPoint.position + Quaternion.Euler(0, attackAngle, 0) * dir);
        Gizmos.DrawLine(user.attackPoint.position, user.attackPoint.position + Quaternion.Euler(0, -attackAngle, 0) * dir);
        Gizmos.DrawLine(user.attackPoint.position, user.attackPoint.position + Quaternion.AngleAxis(attackAngle, Vector3.Cross(dir, Vector3.up)) * dir);
        Gizmos.DrawLine(user.attackPoint.position, user.attackPoint.position + Quaternion.AngleAxis(-attackAngle, Vector3.Cross(dir, Vector3.up)) * dir);
    }
}
