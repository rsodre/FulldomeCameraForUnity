using UnityEngine;

[ExecuteInEditMode]
public class CameraTransformToVFX : MonoBehaviour
{
	public Camera OrthographicCamera;
	//public VisualEffect VFXXX;

    void Awake()
    {
		if (!OrthographicCamera)
			OrthographicCamera = Camera.main;
	}

    void Update()
    {
		if (OrthographicCamera == null)
		{
			Debug.LogWarning("The OrthographicCamera is not set!");
			return;
		}

		if (OrthographicCamera.orthographic == false)
			Debug.LogWarning("The OrthographicCamera is not orthographic!");


	}
}
