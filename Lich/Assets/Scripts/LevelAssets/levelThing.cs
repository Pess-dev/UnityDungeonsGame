using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelThing : MonoBehaviour
{

    private Health health;
    private Animator anim;
   
    void Start()
    {
        anim = GetComponent<Animator>();
        health = GetComponent<Health>(); 
        health.hit.AddListener(Damaged);
    }

    private void Damaged()
    {
        anim.SetTrigger("damaged");
    }

}
