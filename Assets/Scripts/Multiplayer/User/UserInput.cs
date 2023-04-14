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

    public void Turn(InputAction.CallbackContext context)
    {
        turnValue = context.ReadValue<float>();
    }

    public void Drive(InputAction.CallbackContext context)
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
