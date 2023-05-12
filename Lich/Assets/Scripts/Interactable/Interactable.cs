using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string promptMessage;

    [SerializeField]
    protected bool active = false;

    public float cooldown = 1f;
    protected float timer = 0f;

    private Outline outline;

    public UnityEvent interacted;

    private void Awake()
    {
        outline = gameObject.AddComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 0f;
    }

    public void setOutline(bool state)
    {
        outline.OutlineWidth = state ? 10f : 0;
    }

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

    public virtual void Interact(Unit user = null) { interacted.Invoke(); }
}
