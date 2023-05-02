using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    public Unit unit;

    //Look
    public Camera cam; 
    private float xRotation = 0f;
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    //interaction system
    [SerializeField]
    private float interactDistance = 3f;
    private float interactRadius = 1f;
    [SerializeField]
    private LayerMask mask;

    //Interactable item player looking at
    private Interactable visibleItem;

    //altInteract timer
    public float longAltInteract = 0.5f;
    [SerializeField]
    private float timerAltInteract = 0;

    //UI
    [SerializeField]
    private TextMeshProUGUI promptText;

    private void Start()
    {
        if (unit != null)
        {
            unit.setControl();
        }
    }

    private void Update()
    { 
        if (timerAltInteract > 0) timerAltInteract -= Time.deltaTime;
        CheckInteractable();
    }
    private void LateUpdate()
    {
        SyncWithUnit();
    }

    private void toggleCursor(bool visible)
    {
        Cursor.visible = visible;
        if (visible)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    public void SyncWithUnit() 
    {
        if (unit == null)
        { 
            toggleCursor(true);
            return; 
        }

        toggleCursor(false);

        Vector3 eye;

        if (unit.cameraPlace == null)
            eye = unit.transform.position;
        else
            eye = unit.cameraPlace.position;
        cam.transform.position = eye;
        cam.transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles+Vector3.up*( unit.transform.rotation.eulerAngles.y - cam.transform.rotation.eulerAngles.y));
    }

    public void ProcessLook(Vector2 input)
    {
        if (unit == null)
            return; 

        xRotation -= (input.y * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        unit.Rotate(Vector3.up * (input.x * Time.deltaTime) * xSensitivity);

    }

    public void ProcessMove(Vector2 input) 
    {
        if (unit == null)
            return;

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        unit.Move(moveDirection);
    }

    public void Dash(Vector2 input) 
    {
        if (unit == null)
            return;

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        unit.Dash(moveDirection);
    }

    public void Jump()
    {
        if (unit == null)
            return;
        unit.Jump();
    }
    public void Interact()
    {
        if (unit == null || visibleItem == null)
            return;

        visibleItem.BaseInteract();

        if (visibleItem.GetCanGrab()) 
        {
            unit.GrabInteractable(visibleItem);
        }
    }

    public void AltInteractPushed()
    {
        if (unit == null || visibleItem == null)
            return;

        if (visibleItem.GetCanGrab())
        {

        }

        timerAltInteract = longAltInteract;
    }

    public void AltInteractReleased()
    {
        if (unit == null || visibleItem == null)
            return;

        if (visibleItem.GetCanGrab())
        {

        }

        if (timerAltInteract <= 0) unit.ClearItem();
    }

    public void Switch()
    {
        if (unit == null)
            return;

        unit.SwitchItems();
    }

    void CheckInteractable()
    { 
        if (unit == null)
            return;
 
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, interactRadius, interactDistance, mask);

        visibleItem = null;
         
        foreach (RaycastHit hit in hits)
        { 
            if (hit.collider.transform.GetComponent<Interactable>() == null)
                continue;
              
            Interactable item = hit.collider.transform.GetComponent<Interactable>();

            if (!item.getActive())
                continue;
             
            if (visibleItem == null)
            { 
                visibleItem = item;
                continue;
            }
             
            Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Vector3 lastItem = cam.WorldToScreenPoint(visibleItem.transform.position);
            Vector3 currentItem = cam.WorldToScreenPoint(item.transform.position);

            if ((currentItem-center).magnitude< (lastItem- center).magnitude)
                visibleItem = item; 
        }

        Debug.Log("post_contact1");
        if (promptText != null)
            if (visibleItem != null)
                promptText.text = visibleItem.promptMessage;
            else
                promptText.text = "";
       
    }

    private void OnDrawGizmosSelected()
    {
        if (cam != null)
        {
            Gizmos.DrawLine(cam.transform.position, cam.transform.position + cam.transform.forward * interactDistance);
            Gizmos.DrawWireSphere(cam.transform.position, interactRadius);
            Gizmos.DrawWireSphere(cam.transform.position + cam.transform.forward * interactDistance, interactRadius);
        }
    }
}
