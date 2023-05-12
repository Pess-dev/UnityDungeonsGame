using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    public Unit unit;

    //Look
    public Camera cam; 
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    //interaction system
    public float interactionAngle=30;
    private Interactable visibleInteractable;

    //UI
    [SerializeField]
    private TextMeshProUGUI promptText;
    //[SerializeField]
    //private bool UI = true;
    [SerializeField]
    private Animator uiAnim;
    
    private Animator cameraAnim;


    private void Start()
    {
        cameraAnim = GetComponent<Animator>();
        if (unit != null)
        {
            unit.SetControl();
        }
    }

    private void Update()
    { 
        CheckInteractable();
        updateUI(); 
        UpdateAnimation();
    }
    private void LateUpdate()
    {
        SyncWithUnit();
    }

    private void UpdateAnimation()
    {
        if (unit == null)
        {
            cameraAnim.SetBool("dash", false);
        }

        cameraAnim.SetBool("dash", unit.GetDashing());

    }

    private void updateUI()
    {
        uiAnim.SetBool("prompt", promptText.text != "" ? true : false);
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
        Quaternion newRotation = unit.transform.rotation;
        newRotation = Quaternion.Euler(newRotation.eulerAngles + Vector3.right * (-unit.xRotation - newRotation.eulerAngles.x));
        cam.transform.rotation = newRotation;
    }

    public void ProcessLook(Vector2 input)
    {
        if (unit == null)
            return; 
        unit.RotateLocal(Vector3.up * (input.x * Time.deltaTime) * xSensitivity + Vector3.right * (-input.y * Time.deltaTime) * ySensitivity);
    }

    public void ProcessMove(Vector2 input) 
    {
        if (unit == null)
            return;

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        unit.Move(unit.transform.TransformDirection(moveDirection));
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

        visibleInteractable.Interact(unit);
        
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
            Interactable interactable = collision.transform.GetComponentInParent<Interactable>();

            if (interactable == null)
                continue;

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
