using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private PlayerControl control;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        control = GetComponent<PlayerControl>();
        onFoot.Jump.performed += ctx => control.Jump();
        onFoot.Dash.performed += ctx => control.Dash(onFoot.Movement.ReadValue<Vector2>());
        onFoot.Fire.performed += ctx => control.Fire();
        onFoot.Interact.performed += ctx => control.Interact();
        onFoot.AltInteract.performed += ctx => control.AltInteractPushed();
        onFoot.Switch.performed += ctx => control.Switch();
        onFoot.Quit.performed += ctx => control.Pause();


        onFoot.Jump.performed += ctx => control.Skip();
        onFoot.Fire.performed += ctx => control.Skip();
        onFoot.Interact.performed += ctx => control.Skip();

    }
     
    void Update()
    {
        control.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }
    private void FixedUpdate()
    {
        control.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }
    private void OnDisable()
    {
        onFoot.Disable();
    }
}
