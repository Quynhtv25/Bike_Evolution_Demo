Shader "IPS/3D/ColorShadowOutline" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.0, 100)) = 0
	}

	SubShader{
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
		LOD 100
		CGPROGRAM
		#pragma surface surf Lambert

		fixed4 _Color;
		fixed4 _ShadowColor;

		struct Input
		{
			float3 worldNormal;
		};
		
		void surf(Input IN, inout SurfaceOutput o)
		{
			float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
            float NdotL = saturate(dot(IN.worldNormal, lightDir));
            o.Albedo = _Color.rgb + _ShadowColor.rgb * (1.0 - NdotL);
			o.Alpha = _Color.a;
		}
		ENDCG 
        // Outline Pass
        Pass
        {
            Name "OUTLINE1"
            Tags { "RenderType"="Opaque" "Queue"="Geometry"}
            // Tags { "LightMode"="Always" }
            Cull Front
            ZWrite On
            ZTest Less 
            Offset 0, 0

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                float3 viewDir = normalize(mul(UNITY_MATRIX_MV, v.vertex).xyz);
                v.vertex.xyz += viewDir * _OutlineWidth;
                v.vertex.x *= (1 + _OutlineWidth);
                v.vertex.y *= (1 + _OutlineWidth);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
        // Outline Pass
        Pass
        {
            Name "OUTLINE2"
            // Tags { "LightMode"="Always" }
            Cull Back
            ZWrite Off
            ZTest Greater 
            Offset 0, 0

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                float3 viewDir = normalize(mul(UNITY_MATRIX_MV, v.vertex).xyz);
                v.vertex.xyz += viewDir * _OutlineWidth;
                v.vertex.x *= (1 + _OutlineWidth);
                v.vertex.y *= (1 + _OutlineWidth);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
	FallBack "Diffuse"
}