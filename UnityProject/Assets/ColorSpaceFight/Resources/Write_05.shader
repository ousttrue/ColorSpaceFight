Shader "Unlit/NewUnlitShader"
{
	Properties
	{
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(0.5, 0.5, 0.5, 1.0);
			}
			ENDCG
		}
	}
}
