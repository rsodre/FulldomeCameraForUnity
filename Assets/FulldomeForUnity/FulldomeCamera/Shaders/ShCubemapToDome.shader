// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Avante/CubemapToDome" {
    Properties
    {
	    // https://docs.unity3d.com/Manual/SL-Properties.html
        _MainTex("Cubemap Texture", cube) = "" {}
        [Toggle]_IsFulldome ("IsFulldome", int) = 1
        [Toggle]_Masked ("Masked", int) = 0
        _Horizon ("Horizon", float) = 180
        _DomeTilt ("Tilt", float) = 0
        _Rotation ("Rotation", Vector) = (0, 0, 0)
    }
    SubShader {
        Pass{
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			#define texelToUnit2(uv)    ( ((uv) * 2) - float2(1,1) )
			#define texelToUnit3(uv)  	( ((uv) * 2) - float3(1,1,1) )
			#define unitToTexel2(uv)   	( ((uv) + float2(1,1)) * 0.5f )
			#define unitToTexel3(uv)	( ((uv) + float3(1,1,1)) * 0.5f )
			#define QUARTERPI           0.785398163397448f
			#define HALFPI              1.57079632679489661923f
			#define PI                  3.14159265358979323846f
			#define TWOPI           	6.28318530717958647692f

			samplerCUBE _MainTex;
            uniform int _IsFulldome;
			uniform int _Masked;
            uniform float _Horizon;
            uniform float _DomeTilt;
			uniform float3 _Rotation;

			float3x3 rotateAroundX( in float angle )
			{
			    float s = sin(angle);
			    float c = cos(angle);
			    return float3x3(1,0,0,0, c,-s,0,s,c);
			}
			float3x3 rotateAroundY( in float angle )
			{
			    float s = sin(angle);
			    float c = cos(angle);
			    return float3x3(c,0,s, 0,1,0,-s,0,c);
			}
			float3x3 rotateAroundZ( in float angle )
			{
			    float s = sin(angle);
			    float c = cos(angle);
			    return float3x3(c,-s,0, s,c,0, 0,0,1);
			}
			float3x3 makeRotation()
			{
				float x = radians(_Rotation.x);
				float y = radians(_Rotation.y);
				float z = radians(_Rotation.z);
				float ch = cos(y);
			    float sh = sin(y);
			    float ca = cos(z);
			    float sa = sin(z);
			    float cb = cos(x);
			    float sb = sin(x);
				float3x3 cam = float3x3(ch*ca,sh*sb-ch*sa*cb,ch*sa*sb+sh*cb,sa,ca*cb,-ca*sb,-sh*ca,sh*sa*cb+ch*sb,-sh*sa*sb+ch*cb);
			    float3x3 m = cam;
				if (_IsFulldome)
				    m = mul( m, rotateAroundX( -HALFPI ) );
			    if (_DomeTilt > 0)
			        m = mul( m, rotateAroundX( radians(_DomeTilt) ) );
				return m;
			}

            uniform float4 _MainTex_ST;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 m1 : COLOR0;
                float3 m2 : COLOR1;
                float3 m3 : COLOR2;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                // Make rotation matrix
                float3x3 cameraRot = makeRotation();
                o.m1 = cameraRot._m00_m01_m02;
                o.m2 = cameraRot._m10_m11_m12;
                o.m3 = cameraRot._m20_m21_m22;
                return o;
            }

            float3 texelToDome( float2 st, float horizon )
            {
                st = texelToUnit2( st );
                float r = sqrt( st.x * st.x + st.y * st.y );
                float theta = atan2( st.y, st.x );
                float phi = r * (horizon * 0.5f);
                float3 pos;
                pos.x = sin(phi) * cos(theta);
                pos.y = cos(phi);
                pos.z = sin(phi) * sin(theta);
                float y = pos.y; pos.y = pos.z; pos.z = y;  // invert y/z ???
                return pos;
            }
            float3 texelToDome( float2 st )
            {
                return texelToDome( st, PI );
            }

            fixed4 frag(v2f i) : SV_Target
            {
	            if (_Masked)
	            {
		            if ( length(texelToUnit2(i.uv)) > 1)
		            {
		         	   return fixed4(0,0,0,1);
		            }
	            }
                float3 dir = texelToDome(i.uv, radians(_Horizon));
                float3x3 cameraRot = float3x3(i.m1,i.m2,i.m3);
	            dir = mul( cameraRot, dir );
                return texCUBE(_MainTex, dir);
            }
            ENDCG

        }
    }
    Fallback Off
}
