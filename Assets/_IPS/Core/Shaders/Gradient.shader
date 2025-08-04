Shader "IronPirate/Gradient" {
	Properties{
		_MainTex("Main Texture", 2D) = "white" {}
		_TopColor("Top Color", Color) = (1, 1, 1, 1)
		_BottomColor("Bottom Color", Color) = (1, 1, 1, 1)
		_Depth("Depth", Range(0, 1)) = .5
		_Smooth("Smooth", Range(0, 1)) = 0
	}

		SubShader{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			//Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Lambert noforwardadd alpha:fade

			sampler2D _MainTex;
			half4 _TopColor;
			half4 _BottomColor;
			half _Depth;
			half _Smooth;

			struct Input {
				float2 uv_MainTex;
				float3 worldPos;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				half4 c = _TopColor;

				if (IN.uv_MainTex.y < _Depth) {
					c = lerp(_BottomColor, _TopColor, clamp((1 - _Smooth) / _Depth * abs(IN.uv_MainTex.y) + _Smooth, 0, 1));
					c.a = lerp(_BottomColor.a, _TopColor.a, clamp(1/_Depth * abs(IN.uv_MainTex.y), 0, 1));
				}
				o.Albedo = c.rgb * tex2D(_MainTex, IN.uv_MainTex).rgb;
				o.Alpha = c.a;
			}
			ENDCG
		}
		Fallback "Mobile/VertexLit"
}