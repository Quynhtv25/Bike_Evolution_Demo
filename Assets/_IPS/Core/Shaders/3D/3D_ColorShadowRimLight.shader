Shader "IPS/3D/ColorShadowRimLight" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0	}

	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 100
		CGPROGRAM
		#pragma surface surf Lambert

		fixed4 _Color;
		fixed4 _ShadowColor;
        float4 _RimColor;
        float _RimPower;

		struct Input
		{
			float3 worldNormal;
            float3 viewDir;
		};
		
		void surf(Input IN, inout SurfaceOutput o)
		{
			float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
			float NdotL = saturate(dot(IN.worldNormal, lightDir));
			float rim = 1 - saturate(dot(normalize(IN.viewDir), IN.worldNormal));
            o.Albedo = _Color.rgb + _ShadowColor.rgb * (1.0 - NdotL);

			if (IN.viewDir.y > 0) {
				o.Albedo += _RimColor.rgb * pow(rim, _RimPower);
			}

			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "Mobile/Diffuse"
}