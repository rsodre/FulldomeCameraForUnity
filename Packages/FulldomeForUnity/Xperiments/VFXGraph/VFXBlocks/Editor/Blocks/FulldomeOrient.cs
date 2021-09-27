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
	class FulldomeOrient : VFXBlock
	{
		public enum Mode
		{
			Fulldome,
			Fisheye,
		}

		[VFXSetting]
		public Mode mode = Mode.Fulldome;

		public override string name { get { return "Fulldome Orient"; } }

		public override VFXContextType compatibleContexts { get { return VFXContextType.Output; } }
		public override VFXDataType compatibleData { get { return VFXDataType.Particle; } }

		public override IEnumerable<VFXAttributeInfo> attributes
		{
			get
			{
				yield return new VFXAttributeInfo(VFXAttribute.AxisX, VFXAttributeMode.Write);
				yield return new VFXAttributeInfo(VFXAttribute.AxisY, VFXAttributeMode.Write);
				yield return new VFXAttributeInfo(VFXAttribute.AxisZ, VFXAttributeMode.Write);
				yield return new VFXAttributeInfo(VFXAttribute.AngleZ, VFXAttributeMode.ReadWrite);
				yield return new VFXAttributeInfo(VFXAttribute.Position, VFXAttributeMode.Read);
			}
		}

		// Inputs
		protected override IEnumerable<VFXPropertyWithValue> inputProperties
		{
			get
			{
				yield return new VFXPropertyWithValue(new VFXProperty(typeof(Position), "CameraPosition"));
			}
		}

		public override string source
		{
			get
			{
				string outSource = @"
// Rotate to camera
//axisZ = normalize(position - CameraPosition);
//axisX = normalize(cross(GetVFXToViewRotMatrix()[1].xyz,axisZ));
//axisY = cross(axisZ,axisX);

// Face camera
float3x3 viewRot = GetVFXToViewRotMatrix();
axisX = viewRot[0].xyz;
axisY = viewRot[1].xyz;
#if VFX_LOCAL_SPACE // Need to remove potential scale in local transform
axisX = normalize(axisX);
axisY = normalize(axisY);
axisZ = cross(axisX,axisY);
#else
axisZ = -viewRot[2].xyz;
#endif

";
				if (mode == Mode.Fulldome)
					outSource += @"

// Rotate axis
float3 pos = position - CameraPosition;
float azimuth = atan2(pos.z,pos.x);
angleZ += degrees(-azimuth + PI * 0.5);
";
				return outSource;
			}
		}
	}
}

#endif