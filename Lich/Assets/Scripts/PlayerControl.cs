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
    public float interactionAngle=30;
    private Interactable visibleInteractable;

    //UI
    [SerializeField]
    private TextMeshProUGUI promptText;

    private void Start()
    {
        if (unit != null)
        {
            unit.SetControl();
        }
    }

    private void Update()
    { 
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
        unit.setXRotation(xRotation) ;
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

    public void Fire()
    {
        if (unit == null)
            return;

        unit.UseItem();
    }

    public void Interact()
    {
        if (unit == null || visibleInteractable == null)
            return;

        if (visibleInteractable.GetComponent<Item>()) 
        {
            unit.GrabItem(visibleInteractable.transform.GetComponent<Item>());
        }
        else
        {
            visibleInteractable.Interact();
        }
    }

    public void AltInteractPushed()
    {
        if (unit == null)
            return;

        if (unit.CheckGrabbed()) 
            unit.DiscardItem();
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
         
        Collider[] collisions = Physics.OverlapSphere(unit.cameraPlace.position, unit.interactDistance);

        if (visibleInteractable != null)
            visibleInteractable.setOutline(false);

        visibleInteractable = null;

        foreach (Collider collision in collisions)
        {
            if (collision.transform.GetComponent<Interactable>() == null)
                continue;

            Interactable interactable = collision.transform.GetComponent<Interactable>();

            if (!interactable.getActive())
                continue;

            if (Vector3.Angle(collision.transform.position - cam.transform.position, cam.transform.forward) >= interactionAngle)
                continue;


            if (visibleInteractable == null)
            {
                visibleInteractable = interactable;
                continue;
            }

            Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Vector3 lastItem = cam.WorldToScreenPoint(visibleInteractable.transform.position);
            Vector3 currentItem = cam.WorldToScreenPoint(interactable.transform.position);

            if ((currentItem - center).magnitude < (lastItem - center).magnitude)
                visibleInteractable = interactable;

        }

        //RaycastHit hitInfo;
        //if (promptText != null)
        //if (Physics.Raycast(ray, out hitInfo, interactDistance, mask))
        //{
        //    Interactable lookingAtInteractable = hitInfo.collider.transform.GetComponent<Interactable>();
        //    if(lookingAtInteractable == visibleItem)
        //        if (visibleItem != null)
        //            promptText.text = visibleItem.promptMessage;
        //        else
        //            promptText.text = ""; 
        //}
        //    else
        //        promptText.text = "";

        if (visibleInteractable != null)
            visibleInteractable.setOutline(true);

        if (promptText != null)
            if (visibleInteractable != null)
                promptText.text = visibleInteractable.promptMessage;
            else
                promptText.text = "";
    }

    private void OnDrawGizmosSelected()
    {
        if (unit != null)
        { 
            Gizmos.DrawWireSphere(unit.cameraPlace.position, unit.interactDistance);
        }
    }
}
