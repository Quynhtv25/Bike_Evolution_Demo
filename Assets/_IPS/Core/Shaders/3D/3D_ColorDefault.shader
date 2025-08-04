Shader "IPS/3D/ColorDefault" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 100
		CGPROGRAM
		#pragma surface surf Lambert

		fixed4 _Color;

		
		struct Input
		{
			float3 worldNormal;
		};
				
		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = _Color;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}