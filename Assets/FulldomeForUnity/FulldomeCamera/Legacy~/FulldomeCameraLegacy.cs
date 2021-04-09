using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Avante
{
	[ExecuteInEditMode]
	public class FulldomeCameraLegacy : MonoBehaviour
	{
		public Camera mainCamera;
		[EnumFlags]
		public Face cubemapFaces = Face.Everything;
		public Orientation orientation = Orientation.Fulldome;
		[Range(180, 360)]
		public float horizon = 180.0f;
		[Range(0, 45)]
		public float domeTilt = 0.0f;
		public bool masked;
		//public bool renderEquirect = false;
		//public RenderTexture equirectFbo;

		RenderTexture _cubemapFbo;

		static bool _isRenderingCubemap = false;
		static public bool IsRenderingCubemap { get { return _isRenderingCubemap; } }

		bool _IsFulldome { get { return (orientation == Orientation.Fulldome); } }
		Camera _TargetCamera { get { return (mainCamera ? mainCamera : Camera.main); } }
		Transform _Transform { get { return (_TargetCamera != null ? _TargetCamera.transform : transform); } }

		Camera _fulldomeCamera;
		Camera _FulldomeCamera
		{
			get
			{
				if (!_fulldomeCamera)
					_fulldomeCamera = GetComponent<Camera>();
				return _fulldomeCamera;
			}
		}

		Material _cubemapToDomeMaterial;
		Material CubemapToDomeMaterial
		{
			get
			{
				if (!_cubemapToDomeMaterial)
					_cubemapToDomeMaterial = new Material(Shader.Find("Avante/CubemapToDome"));
				return _cubemapToDomeMaterial;
			}
		}

		int _FaceMask
		{
			get
			{
				int mask = (int)cubemapFaces;
				//if (orientation == Orientation.Fulldome)
				//	mask ^= (1 << (int)CubemapFace.NegativeY);
				//else
					//mask ^= (1 << (int)CubemapFace.NegativeZ);
				return mask;
			}
		}

		static FulldomeCameraLegacy _instance;
		public static FulldomeCameraLegacy Instance { get { return _instance; } }

		void Awake()
		{
			Initialize();
		}

		void Start()
		{
		}

		void Initialize()
		{
			if (!_instance)
			{
				_instance = this;
			}
			if (!_cubemapFbo && _FulldomeCamera)
			{
				int size = _FulldomeCamera.targetTexture.width;
				_cubemapFbo = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32)
				{
					dimension = TextureDimension.Cube,
				};
			}
		}

		void Update()
		{
			if (_TargetCamera != null)
			{
				if (Application.isEditor)
					Initialize();
				_FulldomeCamera.depth = _TargetCamera.depth - 1;
			}
		}

		void OnPreRender()
		{
			if (_TargetCamera != null)
			{
				//Debug.Log("FULLDOME CAMERA START");
				_isRenderingCubemap = true;

				// Render cubemap
				//camera.stereoTargetEye = StereoTargetEyeMask.Left;
				_TargetCamera.stereoSeparation = 0;
				_TargetCamera.RenderToCubemap(_cubemapFbo, _FaceMask, Camera.MonoOrStereoscopicEye.Mono);

				//if (renderEquirect && cubemapFbo)
				//cubemapFbo.ConvertToEquirect(equirectFbo, Camera.MonoOrStereoscopicEye.Mono);
			}
		}

		void OnPostRender()
		{
			if (_TargetCamera != null)
			{
				_isRenderingCubemap = false;
				//Debug.Log("FULLDOME CAMERA END");
			}
		}

		// OnRenderImage is called after all rendering is complete to render image
		// https://docs.unity3d.com/ScriptReference/Graphics.Blit.html
		void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			if (_TargetCamera != null)
			{
				CubemapToDomeMaterial.SetInt("_IsFulldome", (_IsFulldome ? 1 : 0));
				CubemapToDomeMaterial.SetFloat("_Horizon", horizon);
				CubemapToDomeMaterial.SetFloat("_DomeTilt", (_IsFulldome ? domeTilt : 0));
				CubemapToDomeMaterial.SetInt("_Masked", (masked ? 1 : 0));
				CubemapToDomeMaterial.SetVector("_Rotation", _Transform.rotation.eulerAngles);
				Graphics.Blit(_cubemapFbo, dest, CubemapToDomeMaterial);
			}
		}

		public RenderTexture GetFulldomeTexture()
		{
			if (!enabled || !_FulldomeCamera.enabled || !gameObject.activeSelf)
				return null;
			return _FulldomeCamera.targetTexture;
		}


		// Gizmo
		FulldomeGizmo _gizmo = null;
		void OnDrawGizmos()
		{
			// Cache gizmo
			if ( _gizmo == null || _gizmo.horizon != horizon )
			{
				_gizmo = new FulldomeGizmo( horizon );
			}

			// Draw
			_gizmo.Draw( _Transform, true, _IsFulldome, domeTilt );
		}
	}
}
