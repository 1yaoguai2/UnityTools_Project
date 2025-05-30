# 雷达扫描涟漪效果 Shader 说明文档

## 效果说明
本Shader实现从屏幕中心向外扩散的多层环形扫描效果，适用于：
- 游戏雷达扫描界面
- 能量护盾特效
- 环境互动水波纹

## 核心参数配置

| 参数名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| _MainTex | 2D纹理 | white | 环形渐变贴图（建议512x512 PNG格式） |
| _Speed | 数值(0-5) | 1 | 波纹移动速度（值越大扩散越快） |
| _WaveCount | 整数(1-10) | 3 | 同时存在的波纹层数 |
| _WaveWidth | 数值(0.01-0.5) | 0.1 | 单层波纹的宽度 |
| _Color | 颜色 | (1,1,1,1) | 波纹颜色（含透明度通道） |

## 使用指南
1. **材质创建**
   ```bash
   # 在Unity中右键创建新材质
   Create > Material > Custom > SpreadEffect > 添加贴图
   ```
2. 基础设置
   - 设置Shader类型为 Custom/SpreadEffect
   - 拖入环形渐变贴图到_MainTex插槽
   - 建议贴图Wrap Mode设置为 Repeat
3. 效果调试建议值
   
   ```
   // 典型雷达扫描配置
   _Speed = 2.5
   _WaveCount = 4
   _WaveWidth = 0.07 
   _Color = RGBA(0, 0.8, 1, 0.6) // 科技
   蓝
   ```
## 进阶技巧
- 添加旋转效果 ：在材质挂载的GameObject上添加旋转脚本
- 多层叠加 ：复制多个材质实例并设置不同的_WaveCount值
- 动态调节 ：通过脚本控制_Speed参数实现扫描加速/减速效果
- 边缘锐化 ：使用黑白分明的环形贴图增强轮廓感

### 代码如下
```Shader
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
```