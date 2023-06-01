using UnityEngine;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[Header("Movement")]
		private float _moveSpeed;
		public float walkSpeed;
		public float sprintSpeed;

		public float groundDrag;

		[Header("Jumping")]
		public float jumpForce;
		public float jumpCooldown;
		public float airMultiplier;
		private bool _readyToJump = true;
		
		[Header("Crouching")]
		public float crouchSpeed;
		public float crouchYScale;
		private float _startYScale;

		[Header("Ground Check")]
		public float playerHeight;

		public LayerMask whatIsGround;
		public bool grounded;
		
		[Header("Slope Handling")]
		public float maxSlopeAngle;
		private RaycastHit _slopeHit;
		private bool _exitingSlope;

		public Transform orientation;

		private InputManager _inputManager;
		private float _horizontalInput;
		private float _verticalInput;

		private Vector3 _moveDirection;

		private Rigidbody _rb;

		public MovementState state;

		public enum MovementState
		{
			Walking,
			Sprinting,
			Crouching,
			Air
		}

		void Start()
		{
			_inputManager = InputManager.Instance;

			_rb = GetComponent<Rigidbody>();
			_rb.freezeRotation = true;

			_startYScale = transform.localScale.y;
		}

		private void FixedUpdate()
		{
			MovePlayer();
		}

		void Update()
		{
			// ground check
			grounded = Physics.Raycast(transform.position, Vector3.down, 1.3f);

			GetInput();
			SpeedControl();
			StateHandler();

			if (grounded)
				_rb.drag = groundDrag;
			else
				_rb.drag = 0;
		}
		
		// ReSharper disable Unity.PerformanceAnalysis
		private void GetInput()
		{
			_horizontalInput = _inputManager.GetPlayerMovement().x;
			_verticalInput = _inputManager.GetPlayerMovement().y;
			
			// jump
			if (_inputManager.PlayerJumpedThisFrame() && _readyToJump && grounded)
			{
				_readyToJump = false;
				
				Jump();
				
				Invoke(nameof(ResetJump), jumpCooldown);
			}
			
			// crouch
			if (_inputManager.PlayerIsCrouchingThisFrame())
			{
				transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
				
				_rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
			}
			// stop crouch
			if (_inputManager.PlayerisNotCrouching())
			{
				transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
			}
		}

		private void StateHandler()
		{
			// crouching
			if (_inputManager.PlayerIsCrouching())
			{
				state = MovementState.Crouching;
				_moveSpeed = crouchSpeed;
			}
			
			// sprinting
			if (grounded && _inputManager.PlayerIsSprinting())
			{
				state = MovementState.Sprinting;
				_moveSpeed = sprintSpeed;
			}
			// walking
			else if (grounded)
			{
				state = MovementState.Walking;
				_moveSpeed = walkSpeed;
			}
			// in the air
			else
			{
				state = MovementState.Air;
			}
		}

		private void MovePlayer()
		{
			// calculate movement direction
			_moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

			// on slope
			if (OnSlope() && !_exitingSlope)
			{
				_rb.AddForce(GetSlopeMoveDirection() * (_moveSpeed * 20f), ForceMode.Force);
				
				if ( _rb.velocity.y > 0)
					_rb.AddForce(Vector3.down * 80f, ForceMode.Force);
			}
			
			// on the ground
			if (grounded)
				_rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
			
			// in the air
			else if (!grounded)
				_rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f * airMultiplier), ForceMode.Force);
			
			// turn gravity off on slope
			_rb.useGravity = !OnSlope();
		}

		private void SpeedControl()
		{
			if (OnSlope() && !_exitingSlope)
			{
				if (_rb.velocity.magnitude > _moveSpeed)
					_rb.velocity = _rb.velocity.normalized * _moveSpeed;
			}
			else
			{
				Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

				if (flatVel.magnitude > _moveSpeed)
				{
					Vector3 limitedVel = flatVel.normalized * _moveSpeed;
					_rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
				}	
			}
		}

		private void Jump()
		{
			_exitingSlope = true;
			
			// reset y velocity
			_rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
			
			_rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
		}

		private void ResetJump()
		{
			_readyToJump = true;

			_exitingSlope = false;
		}

		private bool OnSlope()
		{
			if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 1.5f))
			{
				float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
				return angle < maxSlopeAngle && angle != 0;
			}

			return false;
		}

		private Vector3 GetSlopeMoveDirection()
		{
			return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
		}
	}
}