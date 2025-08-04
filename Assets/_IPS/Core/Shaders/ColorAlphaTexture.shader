// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "IronPirate/Color Alpha Texture" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex("Main Texture", 2D) = "white" {}
}

SubShader {
	Tags { "Queue"="Transparent" "RenderType"="Transparent"}
	LOD 100
	
	Pass {  
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On
		
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		
		struct VertexInput {
			float2 uv : TEXCOORD0;
			half4 vertex : POSITION;
		};
		
		struct VertexOutput {
			float2 uv : TEXCOORD0;
			half4 pos : SV_POSITION;
		};

		sampler2D _MainTex;
		half4 _Color;
		half _Alpha;
		
		VertexOutput vert (VertexInput i)
		{
			VertexOutput o;
			o.uv = i.uv;
			o.pos = UnityObjectToClipPos(i.vertex);
			return o;
		}
		
		half4 frag (VertexOutput i) : COLOR
		{
			_Color.rgb = tex2D(_MainTex, i.uv).rgb;

			return _Color;
		}
		ENDCG
	}
}

}

