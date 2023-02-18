/*
该shader比standardsurfaceshader跟贴近cg编写的初级shader，适合研究
但是缺少灯光是非常可惜的事情
*/


Shader "Unlit/TargetUnlitShader"
{
    Properties
    {
        // 控制贴图的参数
        // 变量名为_MainTex，面板显示为Texture，类型为2D贴图，初始值为white
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // subshader的标签tags
        // 该语句表示渲染类型为用于普通shader的类型
        Tags { "RenderType"="Opaque" }

        // shader的渲染等级标识
        LOD 100

        // subshader中的pass通道，用于承载cg代码
        Pass
        {
            // cg代码开始
            CGPROGRAM

            // 定义顶点着色器 vert
            #pragma vertex vert

            // 定义片元着色器 frag
            #pragma fragment frag

            // make fog work
            // 起雾效果的编译指令
            #pragma multi_compile_fog

            // 包含unity cginc的包
            // 最初级的 unitycg包
            #include "UnityCG.cginc"

            // 创建结构体 appdata
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // 创建结构体 v2f 顶点着色器输出 片元着色器输入
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };


            sampler2D _MainTex;
            float4 _MainTex_ST;

            // 顶点着色器函数
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
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
