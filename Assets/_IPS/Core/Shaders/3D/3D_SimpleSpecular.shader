Shader "IPS/3D/SimpleSpecular"
{
    Properties
    {
        _Color ("Main Color", Color) = (1, 1, 1, 1)
        // _ShadowColor ("Shadow Color", Color) = (0.8, .8, 0.8, 1)
        _SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
        _Gloss ("Glossiness", Range(0, 1)) = 0.8
        _GradientSmoothness ("Gradient Smoothness", Range(0.1, 10)) = 3.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }

            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            float4 _Color;// _ShadowColor;
            float _Gloss;
            float4 _SpecColor;
            float _GradientSmoothness;

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);

                o.worldPos = worldPos;
                o.worldNormal = normalize(worldNormal);
                o.viewDir = viewDir;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 N = normalize(i.worldNormal);
                float3 V = normalize(i.viewDir);
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float3 H = normalize(L + V);

                // Gradient blend
                float height = saturate((i.worldPos.y - _ProjectionParams.y) * _GradientSmoothness);
                float3 baseColor = _Color.rgb;// + _ShadowColor.rgb * (1 - height);

                // Diffuse
                float diff = saturate(dot(N, L));
                baseColor *= (0.5 + 0.5 * diff); // soften diffuse

                // Improved Specular – highlights only bevel
                float specIntensity = pow(max(0, dot(N, H)), 128.0);
                specIntensity *= saturate(dot(N, V));
                specIntensity *= pow(1.0 - abs(dot(N.y, 1)), 1.5); // weaken on flat top
                specIntensity *= _Gloss;

                baseColor += _SpecColor.rgb * specIntensity;

                return float4(baseColor, 1.0);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
