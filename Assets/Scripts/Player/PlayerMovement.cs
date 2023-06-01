using UnityEngine;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[Header("Movement")]
		public float moveSpeed;

		public float groundDrag;

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

		private void GetInput()
		{
			_horizontalInput = _inputManager.GetPlayerMovement().x;
			_verticalInput = _inputManager.GetPlayerMovement().y;
		}

		private void MovePlayer()
		{
			// calculate movement direction
			_moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;


			_rb.AddForce(_moveDirection.normalized * (moveSpeed * 10f), ForceMode.Force);
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
	}
}