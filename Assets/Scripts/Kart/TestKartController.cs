using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpeedType
{
    MPH,
    KPH
}

[RequireComponent(typeof(Rigidbody))]
public class TestKartController : MonoBehaviour
{
    public SpeedType _speedType = SpeedType.MPH;
    private static float _mphMultiplier = 2.23693629f;
    private static float _kphMultiplier = 3.6f;

    [Header("Kart Components"), Space(7)]
    [SerializeField] private GameObject[] _wheelMeshes = new GameObject[4];
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    [SerializeField] private WheelEffects[] _wheelEffects = new WheelEffects[4];
    [SerializeField] private MeshRenderer _brakeLightMeshRenderer;
    [SerializeField] private MeshRenderer _reverseLightMeshRenderer;

    [Header("Kart Settings"), Space(7)]
    [SerializeField] private Vector3 _centerOfMassOffset;
    [SerializeField, Range(20f, 35f)] private float _maximumSteerAngle;
    [SerializeField] private float _fullTorqueOverAllWheels;
    [SerializeField] private float _reverseTorque;
    [SerializeField] private float _topSpeed = 200.0f;
    [SerializeField, Range(0.1f, 1f)] private float _slipLimit;
    [SerializeField] private float _brakeTorque;
    [SerializeField] private float _smoothInputSpeed = 0.2f;

    [Header("Steering Helpers"), Space(7)]
    [SerializeField] private float _antiRollVal = 3500.0f;
    [SerializeField] private float _downForce = 100.0f;
    [SerializeField, Range(0, 1)] private float _steerHelper;
    [SerializeField, Range(0, 1)] private float _tractionControl;

    private Quaternion[] _wheelMeshLocations;
    private float _steerAngle;
    private float _oldRotation;
    private float _currentTorque;
    private Rigidbody _rigidbody;
    private Vector2 _currentInputVector;
    private Vector2 _smoothInputVelocity;
    private int _emission;
    private float _currentMaxSteerAngle;

    public bool Skidding { get; private set;}
    public float BrakeInput { get; private set;}
    public float CurrentSteerAngle { get { return _steerAngle;} }
    public float CurrentSpeed { get { return _speedType == SpeedType.MPH ? _rigidbody.velocity.magnitude * _mphMultiplier : _rigidbody.velocity.magnitude * _kphMultiplier;} }
    public float MaxSpeed { get { return _topSpeed;} }
    public float AccelInput{ get; private set;}

    private void Awake() 
    {
        _wheelMeshLocations = new Quaternion[4];
        for(int i = 0; i < 4; i++)
        {
            _wheelMeshLocations[i] = _wheelMeshes[i].transform.localRotation;
        }

        _rigidbody = GetComponent<Rigidbody>();
        _currentTorque = _fullTorqueOverAllWheels - (_tractionControl * _fullTorqueOverAllWheels);
        _rigidbody.centerOfMass += _centerOfMassOffset;
        _emission = Shader.PropertyToID("_EmissionColor");
    }

    public void Move(float steering, float acceleration, float footBrake)
    {
        Vector2 input = new Vector2(steering, acceleration);
        _currentInputVector = Vector2.SmoothDamp(_currentInputVector, input, ref _smoothInputVelocity, _smoothInputSpeed);
        acceleration = _currentInputVector.y;
        steering = _currentInputVector.x;

        for(int i = 0; i < 4; i++)
        {
            wheelColliders[i].GetWorldPose(out Vector3 position, out Quaternion quaternion);
            _wheelMeshes[i].transform.SetPositionAndRotation(position, quaternion);
        }

        steering = Mathf.Clamp(steering, -1, 1);
        AccelInput = acceleration = Mathf.Clamp(acceleration, 0, 1);
        BrakeInput = footBrake = -1 * Mathf.Clamp(footBrake, -1, 0);

        _steerAngle = steering * _currentMaxSteerAngle;
        wheelColliders[0].steerAngle = wheelColliders[1].steerAngle = _steerAngle;

        SteerHelper();
        ApplyDrive(acceleration, footBrake);
        CapSpeed();
        AddDownForce();
        CheckForWheelSpin();
        TractionControl();
        AntiRoll();
        UpdateSteerAngle();

        Debug.Log($"{CurrentSpeed}");
    }

    private void CapSpeed()
    {
        float speed = _rigidbody.velocity.magnitude;
        switch(_speedType)
        {
            case SpeedType.MPH:
                speed *= _mphMultiplier;
                if(speed > _topSpeed)
                    _rigidbody.velocity = (_topSpeed / _mphMultiplier) * _rigidbody.velocity.normalized;
                break;
            case SpeedType.KPH:
                speed *= _kphMultiplier;
                if(speed > _topSpeed)
                    _rigidbody.velocity = (_topSpeed / _kphMultiplier) * _rigidbody.velocity.normalized;
                break;
        }
    }

    private void ApplyDrive(float acceleration, float footBrake)
    {
        float thrustTorque = acceleration * (_currentTorque / 2f);
        wheelColliders[2].motorTorque = wheelColliders[3].motorTorque = thrustTorque;

        for ( int i = 0; i < 4; i++)
        {
            if ( CurrentSpeed > 5 && Vector3.Angle(transform.forward, _rigidbody.velocity) < 50f)
            {
                wheelColliders[i].brakeTorque = _brakeTorque * footBrake;
            }
            else if (footBrake > 0)
            {
                wheelColliders[i].brakeTorque = 0f;
                wheelColliders[i].motorTorque = -_reverseTorque * footBrake;
            }
        }
    }

    private void SteerHelper()
    {
        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].GetGroundHit(out WheelHit wheelHit);
            if (wheelHit.normal == Vector3.zero) return;
        }

        if(Mathf.Abs(_oldRotation - transform.eulerAngles.y) < 10f)
        {
            float turnAdjust = (transform.eulerAngles.y - _oldRotation) * _steerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnAdjust, Vector3.up);
            _rigidbody.velocity = velRotation * _rigidbody.velocity;
        }

        _oldRotation = transform.eulerAngles.y;
    }

    private void AntiRoll()
    {
        float travelLeft = 1.0f;
        float travelRight = 1.0f;

        // Front AntiRoll Detection & Correction

        bool groundedLeftFront = wheelColliders[0].GetGroundHit(out WheelHit wheelHit);
        if(groundedLeftFront) travelLeft = (-wheelColliders[0].transform.InverseTransformPoint(wheelHit.point).y - wheelColliders[0].radius) / wheelColliders[0].suspensionDistance;

        bool groundedRightFront = wheelColliders[1].GetGroundHit(out wheelHit);
        if(groundedRightFront) travelRight = (-wheelColliders[1].transform.InverseTransformPoint(wheelHit.point).y - wheelColliders[1].radius) / wheelColliders[1].suspensionDistance;

        float antiRollForce = (travelLeft - travelRight) * _antiRollVal;

        if(groundedLeftFront) _rigidbody.AddForceAtPosition(wheelColliders[0].transform.up * -antiRollForce, wheelColliders[0].transform.position);
        if(groundedRightFront) _rigidbody.AddForceAtPosition(wheelColliders[1].transform.up * -antiRollForce, wheelColliders[1].transform.position);

        // Rear AntiRoll Detection & Correction

        bool groundedLeftRear = wheelColliders[2].GetGroundHit(out wheelHit);
        if(groundedLeftRear) travelLeft = (-wheelColliders[2].transform.InverseTransformPoint(wheelHit.point).y - wheelColliders[2].radius) / wheelColliders[0].suspensionDistance;

        bool groundedRightRear = wheelColliders[1].GetGroundHit(out wheelHit);
        if(groundedRightFront) travelRight = (-wheelColliders[3].transform.InverseTransformPoint(wheelHit.point).y - wheelColliders[3].radius) / wheelColliders[1].suspensionDistance;

        antiRollForce = (travelLeft - travelRight) * _antiRollVal;

        if(groundedLeftRear) _rigidbody.AddForceAtPosition(wheelColliders[2].transform.up * -antiRollForce, wheelColliders[2].transform.position);
        if(groundedRightRear) _rigidbody.AddForceAtPosition(wheelColliders[3].transform.up * -antiRollForce, wheelColliders[3].transform.position);
    }

    private void  AddDownForce()
    {
        if (_downForce > 0) _rigidbody.AddForce(_downForce * _rigidbody.velocity.magnitude * -transform.up);
    }

    private void CheckForWheelSpin()
    {
        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].GetGroundHit(out WheelHit wheelHit);

            if (Mathf.Abs(wheelHit.forwardSlip) >= _slipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= _slipLimit)
            {
                // Perform Effects & Play Sound
            }
        }
    }

    private void TractionControl()
    {
        wheelColliders[2].GetGroundHit(out WheelHit wheelHit);
        AdjustTorque(wheelHit.forwardSlip);

        wheelColliders[3].GetGroundHit(out wheelHit);
        AdjustTorque(wheelHit.forwardSlip);
    }

    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= _slipLimit && _currentTorque >= 0)
        {
            _currentTorque -= 10 * _tractionControl;
        }
        else
        {
            _currentTorque += 10 * _tractionControl;
            if (_currentTorque > _fullTorqueOverAllWheels)
            {
                _currentTorque = _fullTorqueOverAllWheels;
            }
        }
    }

    private void UpdateSteerAngle()
    {
        if (CurrentSpeed < 25f)
            _currentMaxSteerAngle = Mathf.MoveTowards(_currentMaxSteerAngle, _maximumSteerAngle, 0.5f);
        else if (CurrentSpeed >= 25f && CurrentSpeed < 60f)
            _currentMaxSteerAngle = Mathf.MoveTowards(_currentMaxSteerAngle, _maximumSteerAngle / 1.5f, 0.5f);
        else if (CurrentSpeed >= 60f)
            _currentMaxSteerAngle = Mathf.MoveTowards(_currentMaxSteerAngle, _maximumSteerAngle / 2f, 0.5f);
    }
}
