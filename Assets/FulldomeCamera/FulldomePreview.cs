using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Avante
{
	// Add this script to your main camera

	[ExecuteInEditMode]
	public class FulldomePreview : MonoBehaviour
	{
		Camera _TargetCamera { get { return GetComponent<Camera>(); } }

		Material _fulldomePreviewMaterial;
		Material FulldomePreviewMaterial
		{
			get
			{
				if (!_fulldomePreviewMaterial)
					_fulldomePreviewMaterial = new Material(Shader.Find("Avante/FulldomePreview"));
				return _fulldomePreviewMaterial;
			}
		}

		RenderTexture _fulldomeTexture;
		int _cullingMask;

		void Start()
		{
			// Allow us to be disabled
		}

		void OnPreCull()
		{
			_fulldomeTexture = null;
			if (!FulldomeCamera.IsRenderingCubemap && FulldomeCamera.Instance)
			{
				_fulldomeTexture = FulldomeCamera.Instance.GetFulldomeTexture();
				if (_fulldomeTexture)
				{
					_cullingMask = _TargetCamera.cullingMask;
					_TargetCamera.cullingMask = 0;
				}
			}
		}

		void OnPreRender()
		{
			if (_fulldomeTexture)
			{
			}
		}

		void OnPostRender()
		{
			if (_fulldomeTexture)
			{
				_TargetCamera.cullingMask = _cullingMask;
			}
		}

		// OnRenderImage is called after all rendering is complete to render image
		// https://docs.unity3d.com/ScriptReference/Graphics.Blit.html
		void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			if (_fulldomeTexture)
			{
				Graphics.Blit(_fulldomeTexture, dest, FulldomePreviewMaterial);
			}
			// Fallback: Just copy
			else
			{
				Graphics.Blit(src, dest);
			}
		}
	}

}
