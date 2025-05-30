Shader "Custom/SpreadEffect" {
    Properties {
        _MainTex ("Ripple Texture", 2D) = "white" {}
        _Speed ("Wave Speed", Range(0, 5)) = 1
        _WaveCount ("Wave Count", Range(1, 10)) = 3
        _WaveWidth ("Wave Width", Range(0.01, 0.5)) = 0.1
        _Color ("Wave Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            float _WaveCount;
            float _WaveWidth;
            fixed4 _Color;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                
                // 计算动态波纹
                float time = _Time.y * _Speed;
                float wave = frac(time - dist * _WaveCount);
                
                // 创建涟漪效果
                float ripple = smoothstep(1 - _WaveWidth, 1, wave) * (1 - saturate(dist * 2));
                fixed4 col = _Color;
                col.a = ripple * _Color.a;

                // 叠加纹理
                fixed4 tex = tex2D(_MainTex, i.uv);
                return col * tex;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}