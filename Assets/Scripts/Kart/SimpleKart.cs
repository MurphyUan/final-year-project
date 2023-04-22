using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleKart : MonoBehaviour
{
    [SerializeField] private float _topSpeed;
    [SerializeField] private GameObject[] _wheelMeshes = new GameObject[4];
    [SerializeField] private WheelCollider[] _wheelColliders = new WheelCollider[4];
    [SerializeField] private int numberOfWheels = 4;

    [SerializeField] private float _torque;

    [SerializeField, Range(20f, 50f)] private float _maximumSteerAngle = 25f;
    private float _steerAngle;
    private float _currentTorque;
    private Rigidbody _rigidbody;

    public bool IsSkidding { get; private set;}
    public float BrakeInput { get; private set;}
    public float SteerAngle { get {return _steerAngle;}}
    public float CurrentSpeed { get { return _rigidbody.velocity.magnitude;}}

    private void Awake() 
    {
        Quaternion[] _wheelMeshLoc = new Quaternion[numberOfWheels];
        for(int i = 0; i < numberOfWheels; i++) 
            _wheelMeshLoc[i] = _wheelMeshes[i].transform.localRotation;

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = Vector3.zero;
        _currentTorque = _torque;
    }

    public void Move(float acceleration, float turnModifier)
    {
        for (int i = 0; i < _wheelMeshes.Length; i++)
        {
            _wheelColliders[i].GetWorldPose(out Vector3 position, out Quaternion quaternion);
            _wheelMeshes[i].transform.SetPositionAndRotation(position, quaternion);
        }

        if(!checkContactGround()) return;

        // _steerAngle = turnModifier * _maximumSteerAngle;
        // _wheelColliders[0].steerAngle = _wheelColliders[1].steerAngle = _steerAngle;

        Turn(turnModifier * _maximumSteerAngle);
        Drive(acceleration);
        if(acceleration <= 0)Brake(acceleration);

        float speed = _rigidbody.velocity.magnitude;
        if(speed > _topSpeed)
            _rigidbody.velocity = _topSpeed * _rigidbody.velocity.normalized;
    }

    private void Drive(float directionModifier)
    {
        float forwardSpeed = directionModifier * _currentTorque;

        forwardSpeed *= directionModifier < 1 ? 2 : 1;

        _rigidbody.velocity += transform.forward * forwardSpeed * Time.deltaTime;

        Debug.Log($"{forwardSpeed} {CurrentSpeed}");
    }

    private void Turn(float turnAngle){
        _wheelColliders[0].steerAngle = _wheelColliders[1].steerAngle = turnAngle;

        Quaternion turnQuat = Quaternion.Euler(Vector3.Scale(Vector3.up,_rigidbody.velocity.normalized) * turnAngle * 2 * Time.deltaTime);

        _rigidbody.MoveRotation(_rigidbody.rotation * turnQuat);

        // float mag = _rigidbody.velocity.magnitude;
        // _rigidbody.velocity = transform.forward * mag;

        // _rigidbody.AddForce((transform.forward * _wheelColliders[0].transform.rotation),ForceMode.Acceleration);
    }

    private void Brake(float acceleration)
    {
        if(acceleration < 0 && CurrentSpeed > 2.5f)
            _wheelColliders[2].brakeTorque = 
            _wheelColliders[3].brakeTorque = 
                _currentTorque * Mathf.Abs(acceleration);
        else 
            _wheelColliders[2].brakeTorque = 
            _wheelColliders[3].brakeTorque = 0;
    }

    private bool checkContactGround()
    {
        foreach(WheelCollider collider in _wheelColliders)
        {
            if(collider.GetGroundHit(out WheelHit wheelHit))
            {
                return true;
            }
        }
        return false;
    }
}
