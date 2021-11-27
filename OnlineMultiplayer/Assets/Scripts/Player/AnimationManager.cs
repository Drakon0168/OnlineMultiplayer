using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class AnimationManager : NetworkBehaviour
{
	private static readonly int movingParameter = Animator.StringToHash("Moving");
	private static readonly int animationSpeedParameter = Animator.StringToHash("Animation Speed");
	private static readonly int velocityParameter = Animator.StringToHash("Velocity");
	private static readonly int jumpingParameter = Animator.StringToHash("Jumping");
	private static readonly int triggerNumberParameter = Animator.StringToHash("Trigger Number");

	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private Transform _meshTransform;
	private Rigidbody _rigidbody;
	private Player _player;

	#region Accessors

	public bool Moving
	{
		get => _animator.GetBool(movingParameter);
		set => _animator.SetBool(movingParameter, value);
	}

	public float AnimationSpeed
	{
		get => _animator.GetFloat(animationSpeedParameter);
		set => _animator.SetFloat(animationSpeedParameter, value);
	}

	public float Velocity
	{
		get => _animator.GetFloat(velocityParameter);
		set => _animator.SetFloat(velocityParameter, value);
	}

	public int Jumping
	{
		get => _animator.GetInteger(jumpingParameter);
		set => _animator.SetInteger(jumpingParameter, value);
	}

	public int TriggerNumber
	{
		get => _animator.GetInteger(triggerNumberParameter);
		set => _animator.SetInteger(triggerNumberParameter, value);
	}
	
	#endregion
	
	#region Unity Overrides

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();

		_animator.fireEvents = false;
		
		_player = GetComponent<Player>();
		_player.OnMoveStart += () => { Moving = true; };
		_player.OnMoveEnd += () => { Moving = false; };

		AnimationSpeed = 1.0f;
	}

	[Client]
	private void Update()
	{
		if (hasAuthority)
		{
			if (Moving)
			{
				Vector3 velocity = _rigidbody.velocity;
				Velocity = new Vector2(velocity.x, velocity.z).magnitude;

				if (velocity.sqrMagnitude > 0.1f)
				{
					_meshTransform.rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.z).normalized);
				}
			}
		}
	}

	#endregion
}