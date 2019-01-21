#include "UnityCG.cginc"

// “multi_compile_instancing”会使你的Shader生成两个Variant，其中一个定义了Shader关键字INSTANCING_ON，另外一个没有定义此关键字。
// 除了这个#pragma指令，下面所列其他的修改都是使用了在UnityInstancing.cginc里定义的宏（此cginc文件位于Unity_Install_Dir\Editor\Data\CGIncludes）。
// 取决于关键字INSTANCING_ON是否被定义，这些宏将展开为不同的代码。
#pragma multi_compile __ UNITY_UI_ALPHACLIP

// 其实是，传说中的 a2v 就是从 Unity App 传过来的数据
struct appdata_t 
{
	// 网格顶点 (一个 UIImage ，其实是两个三角形组成的矩形网格)
	float4 vertex   : POSITION;
	// 顶点的颜色（一般就是白色）
	float4 color    : COLOR;
	// 纹理坐标(俗称 uv)
	float2 texcoord : TEXCOORD0;
	// 用于在Vertex Shader输入 / 输出结构中定义一个语义为SV_InstanceID的元素。
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

// 这个应该熟悉了，是 Vertex Shader to Fragment Shader Data
struct v2f 
{
	// 这个是三维转换为屏幕坐标之后的顶点
	float4 vertex   : SV_POSITION;
	// 顶点颜色
	fixed4 color : COLOR;
	// 纹理坐标（UV）
	float2 texcoord  : TEXCOORD0;
	// 世界坐标系其实是 三维的 vertex 坐标系
	float4 worldPosition : TEXCOORD1;
	// 不用管
	UNITY_VERTEX_OUTPUT_STEREO
};

// 用于计算的纹理 (UIImage 的 Sprite)
uniform sampler2D _MainTex;
			
// 用于采样叠加的纹理(不用管,是固定套路)
uniform fixed4 _TextureSampleAdd;

uniform float4 _MainTex_ST;

// 裁剪区域(不用管)
uniform float4 _ClipRect;

// 顶点着色器(固定套路)
v2f vert(appdata_t IN) 
{
	v2f OUT;
	// 这个宏必须在Vertex Shader的最开始调用，如果你需要在Fragment Shader里访问Instanced属性，
	// 则需要在Fragment Shader的开始也用一下。这个宏的目的在于让Instance ID在Shader函数里也能够被访问到。
	UNITY_SETUP_INSTANCE_ID(IN);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				
	// 缓存三维顶点坐标
	OUT.worldPosition = IN.vertex;
				
	// 三维顶点坐标变换二维顶点坐标
	OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

	OUT.texcoord = IN.texcoord;

	OUT.color = IN.color;
	return OUT;
}

v2f vert_transform_tex(appdata_t IN) 
{
    v2f OUT;
    UNITY_SETUP_INSTANCE_ID(IN)
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT)
    OUT.worldPosition = IN.vertex;
    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                
    OUT.texcoord =  TRANSFORM_TEX(IN.texcoord,_MainTex);
                
    OUT.color = IN.color;
                
    return OUT;
}
		
fixed4 ProcessColor(float2 uv,float4 inColor);

// 片元着色器
fixed4 frag(v2f IN) : SV_Target 
{				
	// 生成灰度颜色(不用考虑  lerp 只看 float4里的就好
	fixed4 color = ProcessColor(IN.texcoord,IN.color);
				
	// 下边就是固定套路
	color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

	#ifdef UNITY_UI_ALPHACLIP
	clip(color.a - 0.001);
	#endif

	return color;
}

// 添加新的函数原型
fixed4 ProcessColor(float2 uv,float4 inColor,float4 worldPosition);

// 片元着色器
fixed4 frag_worldposition(v2f IN) : SV_Target 
{               
    // 生成灰度颜色(不用考虑  lerp 只看 float4里的就好
    fixed4 color = ProcessColor(IN.texcoord,IN.color,IN.worldPosition);
                
    // 下边就是固定套路
    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

    #ifdef UNITY_UI_ALPHACLIP
    clip(color.a - 0.001);
    #endif

    return color;
}