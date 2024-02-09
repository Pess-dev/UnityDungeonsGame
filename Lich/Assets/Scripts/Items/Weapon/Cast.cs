using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cast : Item
{
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
    }

    protected override void Update()
    {
        base.Update();

        UpdateAnimations();
    }
    
    private void UpdateAnimations()
    {
        GetComponent<Animator>().SetFloat("modifier",1-timer/cooldown);
    }

    public override void Use()
    {
        base.Use();
        if (timer > 0)
            return;

        if (castObject == null)
            return;

        //health.Damage(breaking);

        GameObject spawned = ((GameObject)Instantiate(castObject, castPlace.position, castPlace.rotation));

        Vector3 direction = user.Head.forward;

        Ray ray = new Ray(user.Head.position, Quaternion.AngleAxis(-user.xRotation, user.transform.right) * user.transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            direction = (hit.point - castPlace.position).normalized;
        }

        spawned.transform.rotation = Quaternion.LookRotation(direction);

        Debug.DrawLine(castPlace.position, castPlace.position + direction);

        spawned.gameObject.tag = gameObject.tag;
        
        timer = cooldown;
    }

    private void OnDrawGizmosSelected()
    {
    }
}
