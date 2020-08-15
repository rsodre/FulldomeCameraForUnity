//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Experimental.VFX;

//namespace UnityEditor.VFX.Block
//{
//    [VFXInfo(category = "Fulldome For Unity")]
//    class FulldomeWarp : VFXBlock
//    {
//		//// Example parameter
//		//public enum Mode
//		//{
//		//	Awesome,
//		//	Amazing
//		//}
//		//[VFXSetting]
//		//public Mode mode = Mode.Awesome;
		        
//        public override string name { get { return "Fulldome Warp"; } }

//        public override VFXContextType compatibleContexts { get { return VFXContextType.kOutput; } }

//        public override VFXDataType compatibleData { get { return VFXDataType.kParticle; } }

//		// Wahts used from the Graph
//        public override IEnumerable<VFXAttributeInfo> attributes
//        {
//            get
//            {
//				yield return new VFXAttributeInfo(VFXAttribute.Position, VFXAttributeMode.Write);
//            }
//        }

//		// Inputs
//		public class InputProperties
//		{
//			[Range(0f, 1f)]
//			public float Progress = 1f;
//			[Range(0f,360f)]
//			public float Horizon = 180f;
//			public Vector3 CameraPosition = Vector3.zero;
//			//public Vector3 CameraRotation = Vector3.zero;
//		}


//		public override string source
//		{
//			get
//			{
//				return @"

//// HLSL Matrix decompose
//// https://gist.github.com/mattatz/86fff4b32d198d0928d0fa4ff32cf6fa
////float4x4 m = CameraTransform;
////float3 CameraPosition = float3( m[3][0], m[3][1], m[3][2] );

////float3 pos = position;
//float3 pos = position - CameraPosition;

//float mag = length(pos);
//float theta = atan2( sqrt( pos.x * pos.x + pos.z * pos.z ), pos.y);
//float phi = atan2( pos.z, pos.x );
//float r = theta / radians( Horizon / 2.0 );
//pos.xz = float2( r * cos(phi), r * sin(phi) );
//pos.y = mag;

//if (Progress == 1.0)
//	position = pos + CameraPosition;
//else
//	position = lerp(position, pos + CameraPosition, Progress);

//";
//			}
//		}

//	}
//}
