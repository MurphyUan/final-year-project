using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KartController : MonoBehaviour
{
    private Rigidbody sphereRb;

    [SerializeField] private float fwdSpeed;
    [SerializeField] private float revSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;

    private float moveInput;
    private float turnInput;

    private float normalDrag;
    [SerializeField] private float amplifiedDrag;
    [SerializeField] private float alignToGroundTime;

    private void Start() {
        // Get Rigidbody 
        sphereRb = GetComponent<Rigidbody>();
        // Split Motor from rest of Mody
        sphereRb.transform.parent = null;
        // Get current from rigidbody component
        normalDrag = sphereRb.drag;
    }

    private void Update() {

        float newRotation = turnInput * turnSpeed * Time.deltaTime * moveInput;

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

        moveInput *= moveInput > 0 ? fwdSpeed : revSpeed;

        sphereRb.drag = isGrounded ? normalDrag : amplifiedDrag;
    }

    private void FixedUpdate() {
        if(isGrounded)
            sphereRb.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
        else
            sphereRb.AddForce(transform.up * -40f);
    }
}
