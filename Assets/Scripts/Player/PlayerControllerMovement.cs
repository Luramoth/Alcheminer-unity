using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerControllerMovement : MonoBehaviour
	{
		[FormerlySerializedAs("_playerSpeed")] [SerializeField]
		private float playerSpeed = 2.0f;
		[FormerlySerializedAs("_jumpHeight")] [SerializeField]
		private float jumpHeight = 1.0f;
		[FormerlySerializedAs("_gravityValue")] [SerializeField]
		private float gravityValue = -9.81f;
	
		private CharacterController _controller;
		private Vector3 _playerVelocity;
		private bool _groundedPlayer;
		private InputManager _inputManager;
		private Transform _cameraTransform;

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_inputManager = InputManager.Instance;
			_cameraTransform = Camera.main.transform;
		}

		void Update()
		{
			_groundedPlayer = _controller.isGrounded;
			if (_groundedPlayer && _playerVelocity.y < 0)
			{
				_playerVelocity.y = 0f;
			}

			Vector2 movement = _inputManager.GetPlayerMovement();
			Vector3 move = new Vector3(movement.x, 0, movement.y);

			move = new Vector3(1.0f,1.0f,1.0f) * move.z + _cameraTransform.right * move.x;
			move.y = 0;
		
			_controller.Move(move * (Time.deltaTime * playerSpeed));

			// Changes the height position of the player..
			if (_inputManager.PlayerJumpedThisFrame() && _groundedPlayer)
			{
				_playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
			}

			_playerVelocity.y += gravityValue * Time.deltaTime;
			_controller.Move(_playerVelocity * Time.deltaTime);
		}
	}
}