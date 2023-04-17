using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerControls _playerControls;
    private SimpleKart _simpleKart;
    private ResetKart _resetKart;

    private float Acceleration;
    private float Turn;

    private bool isPaused = false;

    public static Action<bool> OnPause;

    private void Awake() 
    {
        _playerControls = new PlayerControls();
        OnEnable();

        _simpleKart = GetComponent<SimpleKart>();
    }

    private void Respawn(InputAction.CallbackContext context)
    {
        // _resetKart.Respawn();
    }

    private void Paused(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;
        OnPause?.Invoke(isPaused);
    }

    private void Update() 
    {
        Acceleration = _playerControls.Player.Drive.ReadValue<float>();
        Turn = _playerControls.Player.Turn.ReadValue<float>();
    }

    private void FixedUpdate() 
    {
        _simpleKart.Move(Acceleration, Turn);
    }

    private void OnEnable() 
    {
        try{
            _playerControls.Player.Enable();
            _playerControls.Player.Respawn.performed += Respawn;
            _playerControls.Player.Pause.performed += Paused;
        }catch(Exception e){
            Debug.Log(e);
        }
    }

    private void OnDisable() 
    {
        try{
            _playerControls.Player.Respawn.performed -= Respawn;
            _playerControls.Player.Pause.performed -= Paused;
            _playerControls.Player.Disable();
        }catch(Exception e){
            Debug.Log(e);
        }
    }
}
