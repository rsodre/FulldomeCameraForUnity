using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

#if THIS_KIND_OF_NODE_HAS_BEEN_DEPRECATED

// Thanks to:
//	http://web.engr.oregonstate.edu/~mjb/WebMjb/Papers/asmedome.pdf
//	https://kineme.net/Discussion/GeneralDiscussion/Fisheyeviewplugin

// Shader Graph vertex displacement tutorial
//	https://www.youtube.com/watch?v=vh85pzT959M

// Custom Dode docs
//	https://docs.unity3d.com/Packages/com.unity.shadergraph@5.3/manual/Custom-Nodes-With-CodeFunctionNode.html

// Coordinate Transformations
//  https://docs.unity3d.com/Packages/com.unity.shadergraph@4.10/manual/Transform-Node.html

[Title("Custom", "FulldomeDisplace")]
class FulldomeDisplace : CodeFunctionNode
{
	public FulldomeDisplace() { name = "Fulldome Displace"; }

	public override bool hasPreview { get { return false; } }

	//public NeededCoordinateSpace RequiresPosition(ShaderStageCapability stageCapability)
	//{
	//	return NeededCoordinateSpace.World;
	//}

	protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("Custom_FulldomeDisplace", BindingFlags.Static | BindingFlags.NonPublic);
    }

	static string Custom_FulldomeDisplace(
		[Slot(0, Binding.WorldSpacePosition)] Vector3 Position,
		[Slot(1, Binding.None)] Vector1 Horizon,
		[Slot(2, Binding.WorldSpacePosition)] out Vector3 Out
	)
	{
        Out = Vector3.zero;
        return @"
{
	float3 Pos = Position;

    //Pos = mul(UNITY_MATRIX_M, float4(Pos, 1)).xyz;
    Pos = mul(UNITY_MATRIX_V, float4(Pos, 1)).xyz;
    Pos = TransformWorldToView(Pos).xyz;

	float rxy = length( Pos.xz );
	if( rxy != 0.0 )
	{
		float phi = atan2( rxy, -Pos.y );
		float lens_radius = phi / ( radians(Horizon) / 2.0 );
		Pos.xz *= ( lens_radius / rxy );
	}

    Pos = mul(UNITY_MATRIX_I_V, float4(Pos, 1)).xyz;
    Pos = mul(UNITY_MATRIX_I_M, float4(Pos, 1)).xyz;
    //Pos = TransformWorldToObject(Pos);

	Out = Pos;
}";
    }
}


// https://gist.github.com/mattatz/86fff4b32d198d0928d0fa4ff32cf6fa

[Title("Custom", "InverseMatrix4")]
class InverseMatrix4 : CodeFunctionNode
{
    public InverseMatrix4() { name = "Inverse Matrix 4"; }

    public override bool hasPreview { get { return false; } }

    //public NeededCoordinateSpace RequiresPosition(ShaderStageCapability stageCapability)
    //{
    //  return NeededCoordinateSpace.World;
    //}

    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("Custom_InverseMatrix4", BindingFlags.Static | BindingFlags.NonPublic);
    }

    static string Custom_InverseMatrix4(
        [Slot(0, Binding.None)] Matrix4x4 In,
        [Slot(1, Binding.None)] out Matrix4x4 Out
    )
    {
        Out = Matrix4x4.identity;
        return @"
{
    float n11 = In[0][0], n12 = In[1][0], n13 = In[2][0], n14 = In[3][0];
    float n21 = In[0][1], n22 = In[1][1], n23 = In[2][1], n24 = In[3][1];
    float n31 = In[0][2], n32 = In[1][2], n33 = In[2][2], n34 = In[3][2];
    float n41 = In[0][3], n42 = In[1][3], n43 = In[2][3], n44 = In[3][3];

    float t11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44;
    float t12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44;
    float t13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44;
    float t14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;

    float det = n11 * t11 + n21 * t12 + n31 * t13 + n41 * t14;
    float idet = 1.0f / det;

    float4x4 ret;

    Out[0][0] = t11 * idet;
    Out[0][1] = (n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44) * idet;
    Out[0][2] = (n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44) * idet;
    Out[0][3] = (n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43) * idet;

    Out[1][0] = t12 * idet;
    Out[1][1] = (n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44) * idet;
    Out[1][2] = (n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44) * idet;
    Out[1][3] = (n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43) * idet;

    Out[2][0] = t13 * idet;
    Out[2][1] = (n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44) * idet;
    Out[2][2] = (n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44) * idet;
    Out[2][3] = (n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43) * idet;

    Out[3][0] = t14 * idet;
    Out[3][1] = (n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34) * idet;
    Out[3][2] = (n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34) * idet;
    Out[3][3] = (n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33) * idet;
}";
    }
}

#endif
