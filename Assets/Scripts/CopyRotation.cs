using UnityEngine;

public class CopyRotation : MonoBehaviour
{
	public Transform copyTransform;
	public Transform targetTransform;

	public bool rotateX;
	public bool rotateY;
	public bool rotateZ;

	private void Start()
	{
		targetTransform = GetComponent<Transform>();
	}

	// Update is called once per frame
	void Update()
	{
		if (rotateX | rotateY | rotateZ)
			targetTransform.rotation = Quaternion.Euler(rotateX ? copyTransform.rotation.eulerAngles.x : targetTransform.rotation.eulerAngles.x,
														rotateY ? copyTransform.rotation.eulerAngles.y : targetTransform.rotation.eulerAngles.y,
														rotateZ ? copyTransform.rotation.eulerAngles.z : targetTransform.rotation.eulerAngles.z);
	}
}