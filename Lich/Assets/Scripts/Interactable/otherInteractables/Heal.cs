using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Interactable
{
    public float hpAdd = 1;

    Health health;
    private void Start()
    {
        health = GetComponent<Health>(); 
    }

    public override void Interact(Unit user = null)
    {
        if (user == null)
            return;

        user.GetComponent<Health>().ChangeHP(hpAdd);

        health.Kill();
    }
}
