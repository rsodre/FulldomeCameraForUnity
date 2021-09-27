using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class PipelineChecker : MonoBehaviour
{
	public enum Pipeline
	{
		StandardRender,
		ScriptableRender,
	}

	public Pipeline TargetPipeline = Pipeline.StandardRender;

	void OnEnable()
    {
#if UNITY_EDITOR
		Pipeline projectPipeline = GraphicsSettings.renderPipelineAsset == null ? Pipeline.StandardRender : Pipeline.ScriptableRender;
		if (projectPipeline != TargetPipeline)
		{
			EditorUtility.DisplayDialog(
					"Invalid Pipeline!",
					"This example was made for the " + TargetPipeline.ToString() + " pipeline, while the project is configured for " + projectPipeline.ToString() + " Pipeline.\n" +
					"Go to Project Settings, Graphics, and " + (TargetPipeline == Pipeline.StandardRender?"remove":"add") + " the Scriptable Render Pipeline Settings asset.",
					"Ok");
		}
#endif
	}
}
