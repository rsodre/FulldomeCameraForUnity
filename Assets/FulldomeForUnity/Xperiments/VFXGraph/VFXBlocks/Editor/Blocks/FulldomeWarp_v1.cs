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
//			[Range(0f,360f)]
//			public float Horizon = 180f;
//			//public Matrix4x4 ModelViewMatrix;
//			//public Matrix4x4 ProjectionMatrix;
//		}

//		public override string source
//		{
//			get
//			{
//				return @"

//float3 pos = position;

////float4x4 ModelViewMatrix = {
////	-1,0,0,0,
////	0,1.2,-1,-2,
////	0,1,-1.2,2.38,
////	0,0,0,1 };
////float4x4 ProjectionMatrix =	{
////	1,0,0,0,
////	0,1,0,0,
////	0,0,-0.2,-1,
////	0,0,0,1 };
////float3 pos = mul( ModelViewMatrix, position );

//// from vertex shader
////float rxy = length( pos.xy );
////if( rxy != 0.0 )
////{
////	float phi = atan2( rxy, -pos.z );
////	float lens_radius = phi / radians( Horizon / 2.0 );
////	pos.xy = pos.xy * ( lens_radius / rxy );
////}

//	float theta = atan2( sqrt( pos.x * pos.x + pos.z * pos.z ), pos.y);	// invert y/z ???
//	float phi = atan2( pos.z, pos.x );
//	float r = theta / radians( Horizon / 2.0 );
//	pos.xz = float2( r * cos(phi), r * sin(phi) );


////position = mul( ProjectionMatrix, pos );

//position = pos;

//";
//			}
//		}

//	}
//}
