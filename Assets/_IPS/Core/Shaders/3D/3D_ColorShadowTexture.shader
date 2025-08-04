Shader "IPS/3D/ColorShadowTexture" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 100
		CGPROGRAM
		#pragma surface surf Lambert

		fixed4 _Color;
		fixed4 _ShadowColor;
		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldNormal;
		};
		
		void surf(Input IN, inout SurfaceOutput o)
		{
			float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
			float NdotL = saturate(dot(IN.worldNormal, lightDir));
			float4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = texColor.rgb * (_Color.rgb + _ShadowColor.rgb * (1.0 - NdotL));
			o.Alpha = texColor.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}