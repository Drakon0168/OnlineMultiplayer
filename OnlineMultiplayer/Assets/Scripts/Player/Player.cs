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
    
    #region Events

    public event System.Action OnMoveStart;
    public event System.Action OnMoveEnd;
    public event System.Action OnJump;
    public event System.Action OnLand;
    
    #endregion

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerControls = GetComponent<PlayerInput>();//new PlayerControls();

        InputActionMap gameplayMap = playerControls.actions.FindActionMap("Gameplay");
        gameplayMap.FindAction("Move").canceled += (context) =>
        {
            Vector2 value = context.ReadValue<Vector2>();
            moveInput = new Vector3(value.x, 0, value.y);
            OnMoveEnd?.Invoke();
        };
        gameplayMap.FindAction("Move").performed += (context) =>
        {
            Vector2 value = context.ReadValue<Vector2>();
            moveInput = new Vector3(value.x, 0, value.y);
            OnMoveStart?.Invoke();
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
