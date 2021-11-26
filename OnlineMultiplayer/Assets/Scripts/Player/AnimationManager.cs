using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
	private static readonly int movingParameter = Animator.StringToHash("Moving");
	private static readonly int animationSpeedParameter = Animator.StringToHash("Animation Speed");
	private static readonly int velocityParameter = Animator.StringToHash("Velocity");
	private static readonly int jumpingParameter = Animator.StringToHash("Jumping");
	private static readonly int triggerNumberParameter = Animator.StringToHash("Trigger Number");

	private Animator _animator;
	private new Rigidbody _rigidbody;
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
		_rigidbody = GetComponentInParent<Rigidbody>();
		_animator = GetComponent<Animator>();
		_player = GetComponentInParent<Player>();

		_player.OnMoveStart += () => { Moving = true; };
		_player.OnMoveEnd += () => { Moving = false; };
	}

	private void Update()
	{
		if (Moving)
		{
			var velocity2D = _rigidbody.velocity;
			Velocity = new Vector2(velocity2D.x, velocity2D.z).magnitude;
		}
	}

	#endregion
}