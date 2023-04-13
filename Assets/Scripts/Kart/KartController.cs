using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour
{
    [SerializeField] Rigidbody sphereRb;

    [SerializeField] private float fwdSpeed;
    [SerializeField] private float revSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;

    private float normalDrag;
    [SerializeField] private float amplifiedDrag;
    [SerializeField] private float alignToGroundTime;

    private void Start() {
        // Split Motor from rest of Mody
        sphereRb.transform.parent = null;
        // Get current from rigidbody component
        normalDrag = sphereRb.drag;
        // Some Change to Update Unity
    }

    public (float forwardValue, float turnValue) MoveKart(float forwardValue, float turnValue){
        float newRotation = turnValue * turnSpeed * Time.deltaTime * forwardValue;

        if (isGrounded)
            transform.Rotate(0, newRotation, 0, Space.World);

        // set karts position to sphere
        transform.position = sphereRb.transform.position;

        // Raycast to the ground and get normal to align kart with it
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);

        //Rotate Kart to align with ground
        Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);

        forwardValue *= forwardValue > 0 ? fwdSpeed : revSpeed;

        sphereRb.drag = isGrounded ? normalDrag : amplifiedDrag;

        return (forwardValue, turnValue);
    }

    public void FixedMoveKart(float forwardValue){
        if(isGrounded)
            sphereRb.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
        else
            sphereRb.AddForce(transform.up * -40f);
    }
}
