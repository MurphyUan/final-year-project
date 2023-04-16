using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(KartController))]
public class PlayerInput : MonoBehaviour
{
    private PlayerControls _playerControls;
    private TestKartController _testKartController;
    private ResetKart _resetKart;

    private float Acceleration;
    private float Turn;

    private bool isPaused = false;

    public static Action<bool> OnPause;

    private void Awake() 
    {
        _playerControls = new PlayerControls();
        OnEnable();
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
        
    }

    private void FixedUpdate() 
    {
        
    }

    private void OnEnable() 
    {
        try{
            _playerControls.Player.Enable();
            _playerControls.Player.Respawn.performed += Respawn;
        }catch(Exception e){
            Debug.Log(e);
        }
    }

    private void OnDisable() 
    {
        try{
            _playerControls.Player.Disable();
        }catch(Exception e){
            Debug.Log(e);
        }
    }
}
