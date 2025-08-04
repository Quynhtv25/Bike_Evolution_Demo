
Shader "IronPirate/FillAmount3D" {
	Properties{
		_MainTex("Main Texture", 2D) = "white" {}
		_BackgroundColor("Background Color", Color) = (1,1,1,1)
		_FillColor("Fill Color", Color) = (1,1,1,1)
		_FillAmount("Fill Amount", Range(0, 1)) = 0
		[KeywordEnum(Horizontal, Vertical)] _Axis("Fill Axis", Int) = 0
		[Toggle]_Reverse("Reverse?", Int) = 0
	}

		SubShader{
			Tags {
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
			}
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			Pass {

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

				struct VertexInput {
					half4 vertex : POSITION;
					half4 uv : TEXCOORD0;
				};

				struct VertexOutput {
					half4 pos : SV_POSITION;
					half4 uv : TEXCOORD0;
				};

				sampler2D _MainTex;
				half4 _BackgroundColor;
				half4 _FillColor;
				half _FillAmount;
				half _Axis;
				half _Reverse;

				VertexOutput vert(VertexInput i) {
					VertexOutput o;
					o.pos = UnityObjectToClipPos(i.vertex);
					o.uv = i.uv;
					return o;
				}

				half4 frag(VertexOutput i) : COLOR{
					half4 result = tex2D(_MainTex, i.uv);
					if ((_Axis == 0 && ((_Reverse == 0 && i.uv.x < _FillAmount) || (_Reverse == 1 && i.uv.x > 1 - _FillAmount)))
						|| (_Axis == 1 && ((_Reverse == 0 && i.uv.y < _FillAmount) || (_Reverse == 1 && i.uv.y > 1 - _FillAmount)))) {
						return result * _FillColor;
					}
					return result * _BackgroundColor;
				}
				ENDCG
			}
	}

}
