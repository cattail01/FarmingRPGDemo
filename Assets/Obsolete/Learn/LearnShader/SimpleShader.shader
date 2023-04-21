Shader "Custom/SimpleShader"
{
	Properties
	{
		
	}
	SubShader
	{
		Pass
		{
			// ...设置渲染状态

			// ...开始编写cg代码
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			void vert(in float4 vertex : POSITION, out float4 position : SV_POSITION)
			{
				position = UnityObjectToClipPos(vertex);
			}

			void frag(in float4 vertex : POSITION, out fixed4 color: SV_TARGET)
			{
				color = fixed4(1, 0, 0, 1);
			}
			// ...cg代码结束
			ENDCG
			
		}
	}
}
