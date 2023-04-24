using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class NetworkInput : NetworkBehaviour
{
    private PlayerControls _playerControls;
    private SimpleKart _kartController;

    private float acceleration;
    private float turn;

    public static Action<bool> OnPause;

    private void Awake() 
    {
        _playerControls = new PlayerControls();
        _kartController = GetComponent<SimpleKart>();
    }

    private void Update() 
    {
        if (!IsOwner) return;
        try{
            acceleration = _playerControls.Player.Drive.ReadValue<float>();
            turn = _playerControls.Player.Turn.ReadValue<float>();
        } catch (Exception e){
            Debug.Log(e);
        }
    }

    private void FixedUpdate() 
    {
        if(!IsOwner) return;
        try{
            _kartController.Move(acceleration, turn);
        } catch (Exception e){
            Debug.Log(e);
        } 
    }

    private void OnEnable()
    {
        try {
            _playerControls.Player.Enable();
            // _playerControls.Player.Respawn.performed += Respawn;
            // _playerControls.Player.Pause.performed += Paused;
        } catch (Exception e){
            Debug.Log(e);
        }
    }

    private void OnDisable()
    {
        try {
            // _playerControls.Player.Respawn.performed -= Respawn;
            // _playerControls.Player.Pause.performed -= Paused;
            _playerControls.Player.Disable();
        } catch (Exception e){
            Debug.Log(e);
        }
    }
}
