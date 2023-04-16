using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(KartController))]
public class PlayerInput : MonoBehaviour
{
    private PlayerControls _playerControls;
    private TestKartController _testKartController;
    private ResetKart _resetKart;

    private float _acceleration;
    private float _turn;

    public float Acceleration {get; private set;}
    public float Turn {get; private set;}

    private bool isPaused = false;

    public bool IsPaused { get; private set;}

    public static Action<bool> OnPause;

    private void Awake() 
    {
        
    }
}
