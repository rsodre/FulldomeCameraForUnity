using UnityEngine;

[ExecuteInEditMode]
public class CameraMatrices : MonoBehaviour
{
	public Matrix4x4 ModelViewMatrix;
	public Matrix4x4 ProjectionMatrix;

	Camera Camera_;

	void Start()
	{
		Camera_ = GetComponent<Camera>();
	}

	void Update()
	{
		if (Camera_)
		{
			ModelViewMatrix = Camera_.cameraToWorldMatrix;
			ProjectionMatrix = Camera_.projectionMatrix;
		}
	}
}
