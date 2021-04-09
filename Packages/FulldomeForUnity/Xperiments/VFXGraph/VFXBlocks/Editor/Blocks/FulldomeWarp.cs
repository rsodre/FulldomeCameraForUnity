#if HAVE_VFX_GRAPH

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

namespace UnityEditor.VFX.Block
{
    [VFXInfo(category = "Fulldome For Unity")]
    class FulldomeWarp : VFXBlock
    {
		//// Example parameter
		//public enum Mode
		//{
		//	Awesome,
		//	Amazing
		//}
		//[VFXSetting]
		//public Mode mode = Mode.Awesome;

		[VFXSetting]
		public bool DebugMode = false;

		public override string name { get { return "Fulldome Warp"; } }

        public override VFXContextType compatibleContexts { get { return VFXContextType.Output; } }

        public override VFXDataType compatibleData { get { return VFXDataType.Particle; } }

		// Wahts used from the Graph
        public override IEnumerable<VFXAttributeInfo> attributes
        {
            get
            {
				yield return new VFXAttributeInfo(VFXAttribute.Position, VFXAttributeMode.Write);
            }
        }

		// Inputs
		public class InputProperties
		{
			[Range(0f, 1f)]
			public float DebugPosition = 1f;
			[Range(0f, 1f)]
			public float DebugDistance = 1f;
			[Range(0f,360f)]
			public float Horizon = 180f;
			public Vector3 CameraPosition = Vector3.zero;
			//public Vector3 CameraRotation = Vector3.zero;
		}
		//protected override IEnumerable<VFXPropertyWithValue> inputProperties
		//{
		//	get
		//	{
		//		yield return new VFXPropertyWithValue(new VFXProperty(typeof(float), "DebugPosition"), 1f);
		//		yield return new VFXPropertyWithValue(new VFXProperty(typeof(float), "DebugDistance"), 1f);
		//		yield return new VFXPropertyWithValue(new VFXProperty(typeof(float), "Horizon"), 180f);
		//	}
		//}


		public override string source
		{
			get
			{
				string setPosition = "pos";
				string setDistance = "mag";
				if (DebugMode)
				{
					setPosition = "lerp(position, pos, DebugPosition)";
					setDistance = "lerp(pos.y, mag, DebugDistance)";
				}
				string src = @"
// Get current particle position
float3 pos = position;

// Get position in World position
#ifndef VFX_WORLD_SPACE
pos = mul(VFXGetObjectToWorldMatrix(),float4(pos,1.0f)).xyz;
#endif

float mag = length(pos);
float theta = atan2( sqrt( pos.x * pos.x + pos.z * pos.z ), pos.y);
float phi = atan2( pos.z, pos.x );
float r = theta / radians( Horizon / 2.0 );
pos.xz = float2( r * cos(phi), r * sin(phi) );
//pos.y = " + setDistance + @";

// move position back to local space
#ifndef VFX_WORLD_SPACE
pos = mul(VFXGetWorldToObjectMatrix(),float4(pos,1.0f)).xyz;
#endif

position = " + setPosition + @";

";
				return src;
			}
		}

	}
}

#endif