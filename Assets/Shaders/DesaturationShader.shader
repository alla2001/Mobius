Shader "Unlit/DesaturationShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _SaturationAmount("Saturation Amount", Range(0, 1)) = 0.5
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _SaturationAmount;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                    fixed4 desaturated = lerp(col, float4(gray, gray, gray, col.a), _SaturationAmount);
                    return desaturated;
                }
                ENDCG
            }
        }
}
