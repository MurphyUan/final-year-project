using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(KartController))]
public class UserInput : MonoBehaviour
{
    [SerializeField] private float speed;

    private Vector2 moveValue;
    private KartController controller;

    private void Start() {
        controller = GetComponent<KartController>();
    }

    public virtual void Move(InputAction.CallbackContext context)
    {
        moveValue = context.ReadValue<Vector2>() * Time.deltaTime * speed;
    }

    private void Update() {
        
    }
}
