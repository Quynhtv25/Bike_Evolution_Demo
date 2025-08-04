Shader "IPS/3D/ColorShadow" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
	}

	SubShader{
		Tags { "RenderType"="Opaque" }
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
	}
	FallBack "Mobile/Diffuse"
}