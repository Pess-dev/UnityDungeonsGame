using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 5f;
    public float lifetime = 10f;
    private float timer = 0f;
    public float knockback = 1f;
    public Vector3 velocity = Vector3.zero;
    private float damagingRadius = 1;

    Health health;
    Rigidbody rb;

    private void Start()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
    }

    public void setVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    public void setDirection(Vector3 direction)
    {
        velocity = direction.normalized * speed;
    }

    private void Update()
    {
        rb.velocity = velocity;

        timer += Time.deltaTime;
        if (timer > lifetime)
            Explode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == gameObject.tag)
            return;
        Explode();
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, damagingRadius);
        foreach (Collider collider in hits)
        {
            if (collider.gameObject.tag == gameObject.tag)
                continue;
            Health targetHealth = collider.transform.GetComponentInParent<Health>();

            if (targetHealth == null)
                continue;

            targetHealth.Damage(damage);

            Rigidbody targetRb = collider.transform.GetComponentInParent<Rigidbody>();

            if (targetRb == null)
                continue;

            Vector3 knockbackDirection = (collider.transform.position - transform.position).normalized;

            targetRb.AddForce(knockbackDirection * knockback, ForceMode.Force);
        }
        health.Kill();
    }
}
