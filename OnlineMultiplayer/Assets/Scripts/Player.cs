using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float moveForce;

    private PlayerInput playerControls;
    private new Rigidbody rigidbody;

    private Vector3 moveInput;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerControls = GetComponent<PlayerInput>();//new PlayerControls();

        InputActionMap gameplayMap = playerControls.actions.FindActionMap("Gameplay");
        gameplayMap.FindAction("Move").canceled += (context) =>
        {
            Vector2 value = context.ReadValue<Vector2>();
            moveInput = new Vector3(value.x, 0, value.y);
        };
        gameplayMap.FindAction("Move").performed += (context) =>
        {
            Vector2 value = context.ReadValue<Vector2>();
            moveInput = new Vector3(value.x, 0, value.y);
        };
    }

    [Client]
    private void Update()
    {
        if (hasAuthority)
        {
            Move();
        }
    }

    private void Move()
    {
        Vector3 targetVelocity = moveInput * speed;
        Vector3 forceDirection = targetVelocity - rigidbody.velocity;
        float forceMagnitude = forceDirection.magnitude;

        if(forceMagnitude > 0)
        {
            forceDirection /= forceMagnitude;
        }

        rigidbody.AddForce(forceDirection * (forceMagnitude / (speed * 2)) * moveForce, ForceMode.Acceleration);

    }
}
