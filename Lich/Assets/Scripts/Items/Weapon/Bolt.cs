using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    public float damage = 5f;
    public float knockback = 1f;
    public float damagingRadius = 1;
    public float ditanceDamaging = 5f;

    private void Start()
    {
        Shot();
        GetComponent<Health>().Kill();
    }


    private void Shot()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit[] hits = Physics.SphereCastAll(ray, damagingRadius, ditanceDamaging);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.tag == gameObject.tag)
                continue;

            Health targetHealth = hit.transform.GetComponentInParent<Health>();

            if (targetHealth == null)
                continue;

            targetHealth.Damage(damage);

            Rigidbody targetRb = hit.transform.GetComponentInParent<Rigidbody>();

            if (targetRb == null)
                continue;

            Vector3 knockbackDirection = (hit.transform.position - transform.position).normalized;

            targetRb.AddForce(knockbackDirection * knockback, ForceMode.Force);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, damagingRadius);
    }
}
