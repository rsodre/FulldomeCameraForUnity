//C# Example
using UnityEditor;
using UnityEngine;
using System;

// Adapted from SAAD KHAWAJA Instant Screenshot
// https://assetstore.unity.com/packages/tools/instant-screenshot-24122

namespace Avante
{
	[Serializable]
	public class ScreenshotConfig : ScriptableObject
	{
		int example;
	}

	[ExecuteInEditMode]
	public class Screenshot : EditorWindow
	{
		public enum Device
		{
			Custom,
			Square1k,
			Square2k,
			Square3k,
			Square4k,
		}

		public ScreenshotConfig config;

		public bool deviceFromAspect = true;
		public Device device = Device.Square2k;

		int resWidth = 2048;
		int resHeight = 2048;

		public Camera myCamera;
		int scale = 1;

		string path = "/CAPTURE";
		//	bool showPreview = true;
		RenderTexture renderTexture;

		bool isTransparent = false;

		// Add menu item named "My Window" to the Window menu
		[MenuItem("Avante/Screenshot Taker")]
		public static void ShowWindow()
		{
			EditorWindow editorWindow = EditorWindow.GetWindow(typeof(Screenshot));
			editorWindow.autoRepaintOnSceneChange = true;
			editorWindow.Show();
			editorWindow.titleContent = new GUIContent("Screenshot");
		}

		float lastTime;

		void OnEnable()
		{
			ScreenshotConfig myInstance = (ScreenshotConfig)Resources.Load("Screenshot.asset") as ScreenshotConfig;

			if (myInstance == null)
			{
				//			myInstance = CreateInstance<ScreenshotConfig>();
				//			AssetDatabase.CreateAsset(myInstance , "Assets/Editor/Instant Screenshot/Resources/Screenshot.asset");
				//			AssetDatabase.SaveAssets();
				//			AssetDatabase.Refresh();
			}

			if (myCamera == null)
				myCamera = Camera.main;
		}

		void OnDisable()
		{
		}

		void OnGUI()
		{
			config = EditorGUILayout.ObjectField(config, typeof(ScreenshotConfig), true, null) as ScreenshotConfig;

			EditorGUILayout.LabelField("Resolution", EditorStyles.boldLabel);

			deviceFromAspect = EditorGUILayout.Toggle("Device From ASpect", deviceFromAspect);
			if (deviceFromAspect)
			{
				if (Camera.main.aspect == 1f)
					device = Device.Square2k;
				else
					device = Device.Custom;
			}

			// Select device
			device = (Device)EditorGUILayout.EnumPopup("Device", device);
			bool customRes = false;
			switch (device)
			{
				case Device.Square1k:
					resWidth = 1024;
					resHeight = 1024;
					break;
				case Device.Square2k:
					resWidth = 2048;
					resHeight = 2048;
					break;
				case Device.Square3k:
					resWidth = 3072;
					resHeight = 3072;
					break;
				case Device.Square4k:
					resWidth = 4096;
					resHeight = 4096;
					break;
				default:
					customRes = true;
					break;
			}
			if (customRes)
			{
				resWidth = EditorGUILayout.IntField("Width", resWidth);
				resHeight = EditorGUILayout.IntField("Height", resHeight);
			}
			else
			{
				EditorGUILayout.LabelField("Width", resWidth.ToString());
				EditorGUILayout.LabelField("Height", resHeight.ToString());
			}

			EditorGUILayout.Space();

			scale = EditorGUILayout.IntSlider("Scale", scale, 1, 15);

			EditorGUILayout.HelpBox("The default mode of screenshot is crop - so choose a proper width and height. The scale is a factor " +
				"to multiply or enlarge the renders without loosing quality.", MessageType.None);


			EditorGUILayout.Space();


			GUILayout.Label("Save Path", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.TextField(path, GUILayout.ExpandWidth(false));
			if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
				path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.HelpBox("Choose the folder in which to save the screenshots ", MessageType.None);
			EditorGUILayout.Space();

			GUILayout.Label("Select Camera", EditorStyles.boldLabel);


			myCamera = EditorGUILayout.ObjectField(myCamera, typeof(Camera), true, null) as Camera;


			isTransparent = EditorGUILayout.Toggle("Transparent Background", isTransparent);


			EditorGUILayout.HelpBox("Choose the camera of which to capture the render. You can make the background transparent using the transparency option.", MessageType.None);

			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("Default Options", EditorStyles.boldLabel);


			if (GUILayout.Button("Set To Screen Size"))
			{
				resHeight = (int)Handles.GetMainGameViewSize().y;
				resWidth = (int)Handles.GetMainGameViewSize().x;

			}


			if (GUILayout.Button("Default Size"))
			{
				resHeight = 1440;
				resWidth = 2560;
				scale = 1;
			}



			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Screenshot will be taken at " + resWidth * scale + " x " + resHeight * scale + " px", EditorStyles.boldLabel);

			if (GUILayout.Button("Take Screenshot", GUILayout.MinHeight(60)))
			{
				if (path == "")
				{
					path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
					Debug.Log("Path Set");
					TakeHiResShot();
				}
				else
				{
					TakeHiResShot();
				}
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Open Last Screenshot", GUILayout.MaxWidth(160), GUILayout.MinHeight(40)))
			{
				if (lastScreenshot != "")
				{
					Application.OpenURL("file://" + lastScreenshot);
					Debug.Log("Opening File " + lastScreenshot);
				}
			}

			if (GUILayout.Button("Open Folder", GUILayout.MaxWidth(100), GUILayout.MinHeight(40)))
			{

				Application.OpenURL("file://" + path);
			}

			if (GUILayout.Button("More Assets", GUILayout.MaxWidth(100), GUILayout.MinHeight(40)))
			{
				Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/publisher/5951");
			}

			EditorGUILayout.EndHorizontal();


			if (takeHiResShot)
			{
				int resWidthN = resWidth * scale;
				int resHeightN = resHeight * scale;
				RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
				RenderTexture rtBackup = myCamera.targetTexture;
				myCamera.targetTexture = rt;

				TextureFormat tFormat;
				if (!isTransparent)
					tFormat = TextureFormat.RGB24;
				else if (myCamera.allowHDR)
					tFormat = TextureFormat.RGBAFloat;
				else
					tFormat = TextureFormat.ARGB32;


				Texture2D screenShot = new Texture2D(resWidthN, resHeightN, tFormat, false);
				myCamera.Render();
				RenderTexture.active = rt;
				screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
				myCamera.targetTexture = rtBackup;
				RenderTexture.active = null;
				byte[] bytes = screenShot.EncodeToPNG();
				string filename = ScreenShotName(resWidthN, resHeightN);

				System.IO.File.WriteAllBytes(filename, bytes);
				Debug.Log(string.Format("Took screenshot to: {0}", filename));
				Application.OpenURL(filename);
				takeHiResShot = false;
			}

			EditorGUILayout.HelpBox("In case of any error, make sure you have Unity Pro as the plugin requires Unity Pro to work.", MessageType.Info);


		}



		private bool takeHiResShot = false;
		public string lastScreenshot = "";


		public string ScreenShotName(int width, int height)
		{
			string prefix = (device == Device.Custom ? string.Format("shot_{0}x{1}", width, height) : device.ToString());
			string date = System.DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
			string strPath = string.Format("{0}/{1}_{2}.png", path, prefix, date);
			lastScreenshot = strPath;

			return strPath;
		}



		public void TakeHiResShot()
		{
			Debug.Log("Taking Screenshot");
			takeHiResShot = true;
		}

	}

}
