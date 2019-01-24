#include "UnityCG.cginc"

// “multi_compile_instancing”会使你的Shader生成两个Variant，其中一个定义了Shader关键字INSTANCING_ON，另外一个没有定义此关键字。
// 除了这个#pragma指令，下面所列其他的修改都是使用了在UnityInstancing.cginc里定义的宏（此cginc文件位于Unity_Install_Dir\Editor\Data\CGIncludes）。
// 取决于关键字INSTANCING_ON是否被定义，这些宏将展开为不同的代码。
#pragma multi_compile __ UNITY_UI_ALPHACLIP

struct appdata_t 
{
    float4 vertex   : POSITION;
    float4 color    : COLOR;
    float2 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f 
{
    float4 vertex   : SV_POSITION;
    fixed4 color : COLOR;
    float2 texcoord  : TEXCOORD0;
    float4 worldPosition : TEXCOORD1;
    float4 grabPosition : TEXCOORD2;
    UNITY_VERTEX_OUTPUT_STEREO
};

uniform sampler2D _MainTex;
uniform fixed4 _TextureSampleAdd;
uniform float4 _ClipRect;

uniform float4 _MainTex_ST;

v2f vert(appdata_t IN) 
{
    v2f OUT;
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
    OUT.worldPosition = IN.vertex;
    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

    // use ComputeGrabScreenPos function from UnityCG.cginc
    // to get the correct texture coordinate
    OUT.grabPosition = ComputeGrabScreenPos(OUT.vertex);

    OUT.texcoord = IN.texcoord;

    OUT.color = IN.color;
    return OUT;
}

// 添加新的函数原型
fixed4 ProcessColor(float2 uv,float4 inColor,float4 worldPosition,float4 grabPosition);

// 片元着色器
fixed4 frag(v2f IN) : SV_Target 
{               
    // 生成灰度颜色(不用考虑  lerp 只看 float4里的就好
    fixed4 color = ProcessColor(IN.texcoord,IN.color,IN.worldPosition,IN.grabPosition);
                
    // 下边就是固定套路
    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

    #ifdef UNITY_UI_ALPHACLIP
    clip(color.a - 0.001);
    #endif

    return color;
}