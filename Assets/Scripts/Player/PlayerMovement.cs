using UnityEngine;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[Header("Movement")]
		public float moveSpeed;

		public float groundDrag;

		public float jumpForce;
		public float jumpCooldown;
		public float airMultiplier;
		private bool _readyToJump = true;

		[Header("Ground Check")]
		public float playerHeight;

		public LayerMask whatIsGround;
		public bool grounded;

		public Transform orientation;

		private InputManager _inputManager;
		private float _horizontalInput;
		private float _verticalInput;

		private Vector3 _moveDirection;

		private Rigidbody _rb;

		void Start()
		{
			_inputManager = InputManager.Instance;

			_rb = GetComponent<Rigidbody>();
			_rb.freezeRotation = true;
		}

		private void FixedUpdate()
		{
			MovePlayer();
		}

		void Update()
		{
			// ground check
			grounded = Physics.Raycast(transform.position, Vector3.down, 1);

			GetInput();
			SpeedControl();

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
		}

		private void MovePlayer()
		{
			// calculate movement direction
			_moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

			// on the ground
			if (grounded)
				_rb.AddForce(_moveDirection.normalized * (moveSpeed * 10f), ForceMode.Force);
			
			// in the air
			else if (!grounded)
				_rb.AddForce(_moveDirection.normalized * (moveSpeed * 10f * airMultiplier), ForceMode.Force);
		}

		private void SpeedControl()
		{
			Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

			if (flatVel.magnitude > moveSpeed)
			{
				Vector3 limitedVel = flatVel.normalized * moveSpeed;
				_rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
			}
		}

		private void Jump()
		{
			// reset y velocity
			_rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
			
			_rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
		}

		private void ResetJump()
		{
			_readyToJump = true;
		}
	}
}