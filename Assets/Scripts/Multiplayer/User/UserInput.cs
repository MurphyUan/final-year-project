using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(KartController))]
public class UserInput : MonoBehaviour
{
    private float forwardValue;
    private float turnValue;

    private KartController controller;

    private void Start() {
        controller = GetComponent<KartController>();
    }

    public virtual void Move(InputAction.CallbackContext context)
    {
        // turnValue = context.ReadValue<Vector2>() * Time.deltaTime * forwardValue;
    }

    public virtual void Turn(InputAction.CallbackContext context)
    {
        turnValue = context.ReadValue<float>();
    }

    public virtual void Drive(InputAction.CallbackContext context)
    {
        forwardValue = context.ReadValue<float>();
    }

    private void Update() 
    {
        (forwardValue, turnValue) = controller.MoveKart(forwardValue, turnValue);
    }

    private void FixedUpdate() 
    {
        controller.FixedMoveKart(forwardValue);
    }
}
