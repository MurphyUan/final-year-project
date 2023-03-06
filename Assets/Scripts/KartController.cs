using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartController : MonoBehaviour
{
    // Uses SphereCollider for Movement Calculations
    [SerializeField] private Rigidbody sphereRigidB;

    // Get User Input - Multiplayer Aspect
    [SerializeField] private InputControls input;

    private float _forwardAmount;
    private float _currentSpeed;

    [SerializeField] private float forwardSpeed;

    // Start is called before the first frame update
    void Start()
    {
        // declare GameObject of Rigidbody
        sphereRigidB.transform.parent = null;
    }

    private void FixedUpdate() 
    {
        sphereRigidB.AddForce(transform.forward * _currentSpeed, ForceMode.Acceleration);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = sphereRigidB.transform.position;

        // _forwardAmount = input.GetForward();
        // horrible code structure, TO BE UPDATED
        // _forwardAmount = Input.GetAxis("Vertical");
        if(_forwardAmount != 0)
            Drive();
    }

    private void Drive()
    {
        _currentSpeed = _forwardAmount *= forwardSpeed;
    }
}
