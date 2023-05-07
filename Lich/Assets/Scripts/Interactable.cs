using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string promptMessage;

    [SerializeField]
    protected bool active = false;

    public float cooldown = 1f;
    protected float timer = 0f;

    private void Update()
    {
        if (timer > 0) 
            timer -= Time.deltaTime;
    }

    public float GetTimer()
    {
        return timer;
    }
    public bool getActive()
    {
        return active;
    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }

    public virtual void Interact() {}
}
