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
        onFoot.Interact.performed += ctx => control.Interact();
    }
     
    void Update()
    {
        control.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
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
