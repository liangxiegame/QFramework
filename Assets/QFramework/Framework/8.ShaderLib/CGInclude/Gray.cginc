#include "UnityCG.cginc"

inline float4 gray(float grayFactor, float4 texColor)
{
    float grayColor = texColor.r * 0.299 + texColor.g * 0.587 + texColor.b * 0.114;

	return lerp(texColor, float4(grayColor, grayColor, grayColor, 1.0), grayFactor);
}