using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(KartController))]
public class UserInput : MonoBehaviour
{
    private float forwardValue;
    private float turnValue;

    private KartController _controller;

    // private void Update() 
    // {
    //     (forwardValue, turnValue) = controller.MoveKart(forwardValue, turnValue);
    // }

    // private void FixedUpdate() 
    // {
    //     controller.FixedMoveKart(forwardValue);
    // }
}
