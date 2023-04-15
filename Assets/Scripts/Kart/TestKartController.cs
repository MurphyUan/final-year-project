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
    [SerializeField] private float _maxHandbrakeTorque;
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
    private Vector2 _smoothInputVector;
    private int _emission;
    private float _currentMaxSteerAngle;

    public bool Skidding { get; private set;}
    public float BrakeInput { get; private set;}
    public float CurrentSteerAngle { get { return _steerAngle;} }
    public float CurrentSpeed { get { return _speedType == SpeedType.MPH ? _rigidbody.velocity.magnitude * 2.23693629f : _rigidbody.velocity.magnitude * 3.6f;} }
    public float MaxSpeed { get { return _topSpeed;} }
    public float AccelInput{ get; private set;}

    private void Awake() {
        
    }
}
