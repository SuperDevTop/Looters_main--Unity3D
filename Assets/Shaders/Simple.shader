Shader "Surfaces/Simple"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		
		[Header(Effects)]
		[Toggle(USE_LIGHTING)]
		_FakeLighting("Lighting", Float) = 0
		
		[Toggle(TRANSPARENCY)]
		_Transparency("Transparency", Float) = 0
		_Visibility("Visibility", Range(0, 1)) = 1

		[Toggle(GRAYSCALE)]
		_GrayscaleToggle("Grayscale", Float) = 0
		_Grayscale("Grayscale Amount", Range(0, 1)) = 0

		[Toggle(BACKGROUND_FILL)]
		_BackgroundFillToggle("Background Fill", Float) = 0

		[Header(Other)]
		[Enum(Two Sided,0, Back,1, Front,2)] _CullMode("Culling Mode", int) = 0
    }
		
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        LOD 100
		Cull[_CullMode]

        Pass
        {
            CGPROGRAM
			#pragma shader_feature USE_LIGHTING
			#pragma shader_feature TRANSPARENCY
			#pragma shader_feature GRAYSCALE
			#pragma shader_feature BACKGROUND_FILL

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float4 color : COLOR;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 color : COLOR;

#ifdef TRANSPARENCY
				float4 screenPos : TEXCOORD2;
#endif
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _Color;

#ifdef TRANSPARENCY
			float _Visibility;
#endif

#ifdef GRAYSCALE
			float _Grayscale;
#endif

#ifdef BACKGROUND_FILL
			float _BackgroundFill;
			float4 _BackgroundFillColor;
#endif

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

#ifdef TRANSPARENCY
				o.screenPos = ComputeScreenPos(o.vertex);
#endif

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 lightNormal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);

#ifdef USE_LIGHTING
					float light = (1 + (lightNormal.y - (lightNormal.z * lightNormal.y)) * 0.2);
#else
					float light = 1;
#endif

				o.color = v.color * light;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color * i.color;

#ifdef GRAYSCALE
				float cDist = length(col);
				col = lerp(col, half4(cDist, cDist, cDist, col.a) * 0.4, _Grayscale);
#endif

#ifdef BACKGROUND_FILL
				col = lerp(col, _BackgroundFillColor, _BackgroundFill);
#endif

#ifdef TRANSPARENCY
				float visibility = i.color.a * _Color.a * _Visibility;
				if (visibility < 1 && _Visibility < 0.99)
				{
					float2 pos = i.screenPos.xy / i.screenPos.w;

					pos *= _ScreenParams.xy; // pixel position
					float4x4 thresholdMatrix =
					{ 1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
						13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
						4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
						16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
					};
					float4x4 _RowAccess = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };
					clip(visibility - thresholdMatrix[fmod(pos.x, 4)] * _RowAccess[fmod(pos.y, 4)]);
				}
#endif

                return col;
            }
            ENDCG
        }
    }
}
