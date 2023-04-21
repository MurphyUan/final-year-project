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
    [SerializeField, Range(0,1)] private float _downForce = 1;

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

        _steerAngle = turnModifier * _maximumSteerAngle;
        _wheelColliders[0].steerAngle = _wheelColliders[1].steerAngle = _steerAngle;

        float speed = _rigidbody.velocity.magnitude;
        if(speed > _topSpeed)
            _rigidbody.velocity = _topSpeed * _rigidbody.velocity.normalized;

        Drive(acceleration);
        if(acceleration <= 0)Brake(acceleration);

        TractionControl();
    }

    private void Drive(float acceleration)
    {
        float motorTorque = acceleration * _currentTorque;
        _wheelColliders[2].motorTorque = _wheelColliders[3].motorTorque = motorTorque;
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

    private void TractionControl()
    {
        _wheelColliders[2].GetGroundHit(out WheelHit wheelHit);
        AdjustTorque(wheelHit.forwardSlip);

        _wheelColliders[3].GetGroundHit(out wheelHit);
        AdjustTorque(wheelHit.forwardSlip);
    }

    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= 1 && _currentTorque >= 0)
        {
            _currentTorque -= 10;
        }
        else
        {
            _currentTorque += 10;
            if (_currentTorque > _torque)
            {
                _currentTorque = _torque;
            }
        }
    }
}
