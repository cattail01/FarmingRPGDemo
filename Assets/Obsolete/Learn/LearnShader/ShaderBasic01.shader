Shader "Custom/ShaderBasic01"	// shader的名称
{
	Properties
	{
		// 此处是材质属性声明
		// 开放到材质面板的属性，也可以叫面板可见变量
	}
	SubShader // 子着色器
	{
		// 此处是定义子着色器的其余代码
		// 顶点-片元着色器
		// 或者表面着色器（旧版本）
		// 或者固定函数着色器

		Pass
		{
			// 此处是定义通道的代码
		}
	}
	// SubShader
	// {
		// 可以编写多个SubShader
		// 至少需要一个
		// 检查第一个是否能运行
		// 如果不能则检查下一个 
	// }

	// ...

	// 如果所有SubShader都不能运行，则返回以下代码
	// 运行指定的基础着色器
	Fallback "ExampleFallbackShader"
}
