using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Avante.HDRP
{
	public enum Orientation
	{
		Fisheye = 0,
		Fulldome = 1,
	}

	public enum Resolution
	{
		Domemaster1k = 1024,
		Domemaster2k = 2048,
		Domemaster3k = 3072,
		Domemaster4k = 4096,
	}

	[Flags]
	public enum Face
	{
		None = 0,
		Everything = 63,
		PositiveX = (1 << CubemapFace.PositiveX),
		NegativeX = (1 << CubemapFace.NegativeX),
		PositiveY = (1 << CubemapFace.PositiveY),
		NegativeY = (1 << CubemapFace.NegativeY),
		PositiveZ = (1 << CubemapFace.PositiveZ),
		NegativeZ = (1 << CubemapFace.NegativeZ),
	}

	[ExecuteInEditMode]
	public class FulldomeCameraHDRP : MonoBehaviour
	{
		public Camera mainCamera;
		[EnumFlags]
		public Face cubemapFaces = Face.Everything;
		public Resolution domemasterResolution = Resolution.Domemaster2k;
		public Orientation orientation = Orientation.Fulldome;
		[Range(180, 360)]
		public float horizon = 180.0f;
		[Range(0, 45)]
		public float domeTilt = 0.0f;
		public bool masked;
		//public bool renderEquirect = false;
		[NonSerialized]
		public RenderTexture cubemapFbo;
		[NonSerialized]
		public RenderTexture domemasterFbo;

		RenderTexture _cubemapFbo;

		bool _IsFulldome { get { return (orientation == Orientation.Fulldome); } }
		Camera _TargetCamera { get { return (mainCamera ? mainCamera : Camera.main); } }
		Transform _Transform { get { return _TargetCamera.transform; } }

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
		void Awake()
		{
			Initialize();
		}

		void Start()
		{
			if (_TargetCamera == null)
				Destroy(this);
		}

		void Initialize()
		{
			if (!_cubemapFbo)
			{
				if (cubemapFbo != null)
					_cubemapFbo = cubemapFbo;
				else
				{
					int size = (int)domemasterResolution;
					_cubemapFbo = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32)
					{
						dimension = TextureDimension.Cube,
					};
				}
			}
		}

		public void LateUpdate()
		{
			if (Application.isEditor)
				Initialize();
			StartCoroutine(RenderFrame());
		}

		IEnumerator RenderFrame()
		{
			yield return new WaitForEndOfFrame();
	
			// Render cubemap
			//camera.stereoTargetEye = StereoTargetEyeMask.Left;
			_TargetCamera.stereoSeparation = 0;
			_TargetCamera.RenderToCubemap(_cubemapFbo, _FaceMask, Camera.MonoOrStereoscopicEye.Mono);

			//if (renderEquirect && cubemapFbo)
			//cubemapFbo.ConvertToEquirect(equirectFbo, Camera.MonoOrStereoscopicEye.Mono);

			CubemapToDomeMaterial.SetInt("_IsFulldome", (_IsFulldome ? 1 : 0));
			CubemapToDomeMaterial.SetFloat("_Horizon", horizon);
			CubemapToDomeMaterial.SetFloat("_DomeTilt", (_IsFulldome ? domeTilt : 0));
			CubemapToDomeMaterial.SetInt("_Masked", (masked ? 1 : 0));
			CubemapToDomeMaterial.SetVector("_Rotation", _Transform.rotation.eulerAngles);
			Graphics.Blit(_cubemapFbo, domemasterFbo, CubemapToDomeMaterial);
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
