Shader "Unlit/DisplacementShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DisplacementTex ("DisplacementTex", 2D) = "white" {}
		_MaskTex ("MaskTex", 2D) = "white" {}
		_Movement ("Movement", float) = 1
		_WaterColor ("WaterCol", Color) = (0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members worldVertex)
#pragma exclude_renderers d3d11
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float2 worldVertex : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _DisplacementTex;
			sampler2D _MaskTex;
			float4 _MainTex_ST;
			fixed4 _MainTex_TexelSize;
			fixed _Movement;
			fixed4 _WaterColor;
			
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVertex = v.vertex.xy;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed mask = tex2D(_MaskTex, i.uv).r;

				fixed2 dPos = i.uv;
				dPos.x += (_Time[0]/10) % 2;
				dPos.y += (_Time[0]/10) % 2;
				fixed4 disp = tex2D(_DisplacementTex, dPos);
				i.uv.x += disp.x * 0.006 * mask * _Movement;
				i.uv.y += disp.y * 0.006 * mask * _Movement;

				fixed4 col = tex2D(_MainTex, fixed2(i.uv.x, i.uv.y)); 
				fixed waterBlend = mask * 0.5 * _WaterColor.a;
				col.r = lerp(col.r, _WaterColor.r, waterBlend);
				col.g = lerp(col.g, _WaterColor.g, waterBlend);
				col.b = lerp(col.b, _WaterColor.b, waterBlend);

				return col;
			}
			ENDCG
		}
	}
}
