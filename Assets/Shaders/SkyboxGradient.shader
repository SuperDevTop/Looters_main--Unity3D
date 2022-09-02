Shader "Skybox/Skybox Gradient" {
	Properties {
		_Color ("Color", COLOR) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags {
			"Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox"
		}
		Cull Off ZWrite Off

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define clamp01(v) clamp(v, 0, 1)

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal: NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 posOS : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;

			uniform int _GradientKeyCount;
			uniform fixed4 _GradientColors[16];
			uniform half _GradientKeys[16];
			uniform half _GradientOffset;

			inline float2 ToRadialCoords(float3 coords)
	        {
	            float3 normalizedCoords = normalize(coords);
	            float latitude = acos(normalizedCoords.y);
	            float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
	            float2 sphereCoords = float2(longitude, latitude) * float2(0.5/UNITY_PI, 1.0/UNITY_PI);
	            return float2(0.5,1.0) - sphereCoords;
	        }

			half map(half value, half min1, half max1, half min2, half max2)
			{
				return lerp(min2, max2, (value - min1) / (max1 - min1));
			}

			fixed2 map2(fixed2 value, half min1, half max1, half min2, half max2)
			{
				return fixed2(map(value.x, min1, max1, min2, max2), map(value.y, min1, max1, min2, max2));
			}

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = map2(TRANSFORM_TEX(v.uv, _MainTex), -1, 1, 0, 1);
				o.uv.y += _GradientOffset;
				o.uv.y = clamp01(o.uv.y);
				o.posOS = v.vertex;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float t = i.uv.y;
				for (int i = 0; i < _GradientKeyCount - 1; i++) {
					float t1 = _GradientKeys[i];
					float t2 = _GradientKeys[i + 1];
					if (t < t2) {
						return lerp(
							_GradientColors[i],
							_GradientColors[i + 1],
							(t - t1) / (t2 - t1));
					}
				}
				return _GradientColors[_GradientKeyCount - 1];
			}
			ENDCG
		}
	}
}