using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float sprintSpeed;
    [SerializeField]
    private float moveForce;
    private bool sprinting = false;

    private PlayerInput playerControls;
    private new Rigidbody rigidbody;

    private Vector3 moveInput;

    public float Speed
    {
        get
        {
            if (sprinting)
            {
                return sprintSpeed;
            }

            return walkSpeed;
        }
    }
    
    #region Events

    public event System.Action OnMoveStart;
    public event System.Action OnMoveEnd;
    public event System.Action OnSprintStart;
    public event System.Action OnSprintEnd;
    public event System.Action OnJump;
    public event System.Action OnLand;

    #endregion

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerControls = GetComponent<PlayerInput>();//new PlayerControls();

        InputActionMap gameplayMap = playerControls.actions.FindActionMap("Gameplay");
        gameplayMap.FindAction("Move").performed += UpdateMove;
        gameplayMap.FindAction("Move").canceled += UpdateMove;
        gameplayMap.FindAction("Sprint").performed += ToggleSprint;
        gameplayMap.FindAction("Sprint").canceled += ToggleSprint;
        
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
        Vector3 targetVelocity = moveInput * Speed;
        Vector3 forceDirection = targetVelocity - rigidbody.velocity;
        float forceMagnitude = forceDirection.magnitude;

        if(forceMagnitude > 0)
        {
            forceDirection /= forceMagnitude;
        }

        rigidbody.AddForce(forceDirection * (forceMagnitude / (Speed * 2)) * moveForce, ForceMode.Acceleration);
    }

    private void UpdateMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        moveInput = new Vector3(value.x, 0, value.y);
        OnMoveStart?.Invoke();
    }
    
    private void ToggleSprint(InputAction.CallbackContext context)
    {
        if (sprinting)
        {
            sprinting = false;
            OnSprintEnd?.Invoke();
        }
        else
        {
            sprinting = true;
            OnSprintStart?.Invoke();
        }
    }
}
