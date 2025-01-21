Shader "Unlit/GuangGaoPai"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    [MaterialToggle]_Verical("Vercial",Range(0,1))=1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent""Quene"="Transparent" "IgnoreProjector"="True"
		"DisableBacthing"="True"//关闭合批
		}
        LOD 100
        Pass
        {
            Zwrite off//不将此对象的像素写入深度缓冲区
			Blend SrcAlpha OneMinusSrcAlpha//混合要渲染像素的A通道和1-要渲染的像素的A通道
			//禁用剔除，绘制所有面
			Cull off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed _Verical;
            v2f vert (appdata v)
            {
                v2f o;
				float3 center = float3(0,0,0);
				//视角方向：摄像机的坐标减去物体的点
				float3 view = mul(unity_WorldToObject,float4(_WorldSpaceCameraPos,1));
				float3 normalDir = view - center;
				//表面法线的变化：如果_Verical=1，则为表面法线，否则为向上方向
				normalDir.y = normalDir.y*_Verical;
				//归一化
				normalDir = normalize(normalDir);
				float3 upDir = abs(normalDir.y) > 0.999 ? float3(0, 0, 1) : float3(0, 1, 0);
				//叉乘  cross(A,B)返回两个三元向量的叉积(cross product)。注意，输入参数必须是三元向量
				float3 rightDir = normalize(cross(upDir,normalDir));
				upDir = normalize(cross(normalDir, rightDir));
				//计算中心点偏移
				float3 centerOffs = v.vertex.xyz - center;
				//位置的变换
				float3 localPos = center + rightDir * centerOffs.x+upDir*centerOffs.y+normalDir* centerOffs.z;
                o.vertex = UnityObjectToClipPos(localPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
