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

        //Lights - Brake Lights
        // if (footBrake > 0)
        // {
        //     if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, _rigidbody.velocity) < 50f)
        //     {
        //         TurnBrakeLightsOn();
        //     }
        //     else
        //     {
        //         TurnBrakeLightsOff();
        //     }
        // }
        // else
        // {
        //     TurnBrakeLightsOff();
        // }
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
}
