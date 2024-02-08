using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private PlayerControl control;    

    public Vector2 lookInput {get; private set;}
    
    public Vector2 moveInput {get; private set;}

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        control = GetComponent<PlayerControl>();
        onFoot.Jump.performed += ctx => control.Jump();
        onFoot.Dash.performed += ctx => control.Dash();
        onFoot.Fire.performed += ctx => control.Fire();
        onFoot.Interact.performed += ctx => control.Interact();
        onFoot.AltInteract.performed += ctx => control.AltInteractPushed();
        onFoot.Switch.performed += ctx => control.Switch();
        onFoot.Quit.performed += ctx => control.Pause();

        onFoot.Jump.performed += ctx => control.Skip();
        onFoot.Fire.performed += ctx => control.Skip();
        onFoot.Interact.performed += ctx => control.Skip();
        onFoot.OneTap.performed += ctx => control.Skip();
        
        // InputSystem.onDeviceChange += (device, change) =>
        // {
        //     switch (change)
        //     {
        //         case InputDeviceChange.Added:
        //             Debug.Log("Device added: " + device);
        //             break;
        //         case InputDeviceChange.Removed:
        //             Debug.Log("Device removed: " + device);
        //             break;
        //         case InputDeviceChange.ConfigurationChanged:
        //             Debug.Log("Device configuration changed: " + device);
        //             break;
        //     }
        // };
    }
     
    void Update()
    {
        control.ProcessMove(onFoot.Movement.ReadValue<Vector2>()+moveInput);
    }

    private void FixedUpdate()
    {
        control.ProcessLook(onFoot.Look.ReadValue<Vector2>()+lookInput);
        lookInput= Vector2.zero;
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }
    private void OnDisable()
    {
        onFoot.Disable();
    }


    public void SetLookDelta(Vector2 delta){
        lookInput = delta;
    }

    public void SetMoveDelta(Vector2 delta){
        moveInput = delta;
    }
}
