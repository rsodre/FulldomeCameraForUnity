#if HAVE_VFX_GRAPH

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.VFX.Block
{
	//
	// Based on com.unity.visualeffectgraph@5.16.1-preview/Editor/Models/Blocks/Implementations/Orientation/Orient.cs
	// LookAtPosition mode
	//

	[VFXInfo(category = "Fulldome For Unity")]
	class SizeOverDistance : VFXBlock
	{
		public enum Mode
		{
			Fulldome,
			Fisheye,
		}

		public override string name { get { return "Set Size Over Distance"; } }

		public override VFXContextType compatibleContexts { get { return VFXContextType.Output; } }
		public override VFXDataType compatibleData { get { return VFXDataType.Particle; } }

		public override IEnumerable<VFXAttributeInfo> attributes
		{
			get
			{
				yield return new VFXAttributeInfo(VFXAttribute.Size, VFXAttributeMode.Write);
				yield return new VFXAttributeInfo(VFXAttribute.Position, VFXAttributeMode.Read);
			}
		}

		// Inputs
		public class InputProperties
		{
			public AnimationCurve SizeCurve = AnimationCurve.EaseInOut(0,1,1,0);
			[Range(0f, 100f)]
			public float MinDistance = 0f;
			[Range(0f, 100f)]
			public float MaxDistance = 100f;
			public Vector3 CameraPosition = Vector3.zero;
		}
		//protected override IEnumerable<VFXPropertyWithValue> inputProperties
		//{
		//	get
		//	{
		//		yield return new VFXPropertyWithValue(new VFXProperty(typeof(AnimationCurve), "SizeCurve"));
		//		yield return new VFXPropertyWithValue(new VFXProperty(typeof(Position), "CameraPosition"));
		//	}
		//}

		public override string source
		{
			get
			{
				string outSource = @"
float3 pos = position - CameraPosition;
float mag = length(pos);
//float mag = pos.y;
float t = smoothstep(MinDistance,MaxDistance,mag);
size = SampleCurve(SizeCurve, t);
";
				return outSource;
			}
		}
	}
}

#endif
