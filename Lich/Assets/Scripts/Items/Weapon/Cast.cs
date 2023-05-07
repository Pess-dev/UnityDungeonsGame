using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cast : Item
{
    [SerializeField]
    private Object castObject;
    
    [SerializeField]
    private Transform castPlace;

    Unit user;

    protected override void Update()
    {
        base.Update();
    }
    public override void Release()
    {
        base.Release();
        user = null;
    }
    public override void Use(Unit unit)
    {
        if (timer > 0)
            return;

        Projectile spawned = ((GameObject)Instantiate(castObject, castPlace.position, castPlace.rotation)).GetComponent<Projectile>();
        

        user = unit;
        timer = cooldown;
        damaged.Clear();
    }
}
