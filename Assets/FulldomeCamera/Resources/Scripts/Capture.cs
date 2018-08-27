using UnityEngine;
using System.Collections;
using System.IO;

namespace Avante
{
	public class Capture : MonoBehaviour
	{
		public enum FileType
		{
			PNG,
			JPG
		}

		public RenderTexture fbo;
		public FileType fileType = FileType.PNG;
		[Range(0, 100)]
		public int jpgQuality = 75;
		public bool isTransparent;
		public string capturePath = "/CAPTURE";
		public string filePrefix = "Capture";
		public int framerate = 30;
		public float totalSecods = 1;
		public float skipFrames = 0;
		public bool capturing = false;

		Camera _camera;
		Texture2D _tex;

		bool _captureNow;
		int _frameNumber;
		int _totalFrames;
		int _w;
		int _h;

		void Awake()
		{
			Application.runInBackground = true;

			if (capturing)
			{
				_totalFrames = (int)(totalSecods * framerate);
				Time.captureFramerate = framerate;
				if (!Directory.Exists(capturePath))
					Directory.CreateDirectory(capturePath);
			}
		}

		void Start()
		{
			if (capturing)
			{
				_camera = GetComponent<Camera>();
				_camera.targetTexture = fbo;
				_w = fbo.width;
				_h = fbo.height;

				TextureFormat tFormat;
				if (!isTransparent)
					tFormat = TextureFormat.RGB24;
				else if (_camera.allowHDR)
					tFormat = TextureFormat.RGBAFloat;
				else
					tFormat = TextureFormat.ARGB32;

				_tex = new Texture2D(_w, _h, tFormat, false);   // HDR
			}
		}

		void Update()
		{
			if (capturing)
			{
				if (Time.time > totalSecods)
				{
					capturing = false;
					Debug.Log("Finished Capture!!!");
				}
				else
					_captureNow = true;
			}
			else if (Input.GetKeyDown(KeyCode.Space))
			{
				_captureNow = true;
			}
		}

		void OnPostRender()
		{
			if (_captureNow)
			{
				if (_frameNumber < skipFrames)
				{
					Debug.Log("Skipping frame " + _frameNumber + " of " + _totalFrames);
				}
				else
				{
					if (!Directory.Exists(capturePath))
						Directory.CreateDirectory(capturePath);

					RenderTexture.active = fbo;
					_tex.ReadPixels(new Rect(0, 0, _w, _h), 0, 0);
					_tex.Apply();

					string fullName = MakeName();
					byte[] bytes = (fileType == FileType.PNG ? _tex.EncodeToPNG() : _tex.EncodeToJPG(jpgQuality));
					File.WriteAllBytes(fullName, bytes);

					Debug.Log("Captured frame " + _frameNumber + " of " + _totalFrames + " [" + fullName + "]");
				}

				_frameNumber++;
				_captureNow = false;
			}
		}

		string MakeName()
		{
			//string pathName = Application.dataPath + "../CAPTURE/";
			string fullName = capturePath + "/" + filePrefix + "_" + Lib.ToStringZeroes(_frameNumber, 4) + "." + fileType.ToString().ToLower();
			return fullName;
		}
	}	
}
