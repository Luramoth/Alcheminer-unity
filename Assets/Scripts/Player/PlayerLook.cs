using UnityEngine;

namespace Player
{
	public class PlayerLook : MonoBehaviour
	{
		public float sensX;
		public float sensY;

		[SerializeField]
		private Transform orientation;
		
		private InputManager _inputManager;

		private float _xRotation;
		private float _yRotation;

		// Start is called before the first frame update
		void Start()
		{
			_inputManager = InputManager.Instance;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		// Update is called once per frame
		void Update()
		{
			// mouse input
			float mouseX = _inputManager.GetMouseDelta().x * Time.deltaTime * sensX;
			float mouseY = _inputManager.GetMouseDelta().y * Time.deltaTime * sensY;

			_yRotation += mouseX;
			
			_xRotation -= mouseY;
			_xRotation = Mathf.Clamp(_xRotation, -90, 90);
			
			// rotate cam and body
			transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
			orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
		}
	}
}