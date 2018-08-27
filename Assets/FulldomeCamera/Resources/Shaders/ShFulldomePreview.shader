// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Avante/FulldomePreview" {
    Properties
    {
        _MainTex("Fulldome Texture", 2D) = "" {}
    }
    SubShader {
        Pass{
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float4 _MainTex_ST;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f i) : SV_Target
            {
                float2 st = i.uv;
            	float aspect = (_ScreenParams.x / _ScreenParams.y);
            	if ( aspect > 1.0f)
            	{
            		st.x = (st.x * aspect) - (aspect - 1.0f) / 2.0f;
            		if (st.x < 0 || st.x > 1)
            			return fixed4(0,0,0,1);
            	}
            	else if ( aspect < 1.0f)
            	{
            		st.y = (st.y / aspect) - ((1.0f/aspect) - 1.0f) / 2.0f;
            		if (st.y < 0 || st.y > 1)
            			return fixed4(0,0,0,1);
            	}
                return tex2D(_MainTex, st);
            }
            ENDCG

        }
    }
    Fallback Off
}
