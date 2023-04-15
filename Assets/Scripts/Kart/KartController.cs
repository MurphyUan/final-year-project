using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartController : MonoBehaviour
{
    [SerializeField] Rigidbody sphereRb;
    [SerializeField] Rigidbody modelRb;

    [SerializeField] private float MaxSpeed;

    [SerializeField] private float fwdSpeed;
    [SerializeField] private float revSpeed;
    [SerializeField] private float turnSpeed;

    [SerializeField] private LayerMask groundLayer;
    
    [SerializeField] private float amplifiedDrag;
    [SerializeField] private float alignToGroundTime;

    private bool isGrounded;
    private float normalDrag = 1;

    private float forwardValue;

    private float forwardContextValue = 0;
    private float turnContextValue = 0;

    private void Start() {
        // Split Motor and Model from the Kart
        sphereRb.transform.parent = null;
        modelRb.transform.parent = null;
        // Get current from rigidbody component
        normalDrag = sphereRb.drag;
        // Some Change to Update Unity
    }

    public void Turn(InputAction.CallbackContext context)
    {
        Debug.Log($"{context.ReadValue<float>()}");

        turnContextValue = context.ReadValue<float>();
    }

    public void Drive(InputAction.CallbackContext context)
    {
        Debug.Log($"{context.ReadValue<float>()}");
        forwardContextValue = context.ReadValue<float>();
    }

    private void Update() {
        float turnValue = turnContextValue * turnSpeed * Time.deltaTime;

        if(isGrounded)
        {
            transform.Rotate(0, turnValue, 0, Space.World);
        }

        transform.position = sphereRb.transform.position;
        sphereRb.drag = isGrounded ? normalDrag : amplifiedDrag;

        forwardValue = forwardContextValue > 0 ? forwardContextValue * fwdSpeed : forwardContextValue * revSpeed;

        Debug.Log($"tV: {turnValue}, tC: {turnContextValue}, fV: {forwardValue}, fC: {forwardContextValue}");

        if(isGrounded)return;

        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);

        //Rotate Kart to align with ground
        Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);
    }

    private void FixedUpdate() {
        if(isGrounded){
            sphereRb.AddForce(transform.forward * forwardValue, ForceMode.Acceleration);
        }
        else
            sphereRb.AddForce(transform.up * -40f);
        modelRb.MoveRotation(transform.rotation);
    }
}
