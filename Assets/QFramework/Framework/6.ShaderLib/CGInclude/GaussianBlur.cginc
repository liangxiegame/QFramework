
uniform float blurKernel[9];
static float GaussianKernel[9] = 
{ 
    0.0947416f, 0.118318f, 0.0947416f,
    0.118318f, 0.147761, 0.118318f,
    0.0947416f, 0.118318f, 0.0947416f
};


fixed4 GaussianBlur(sampler2D mainTex,float2 uv,float4 inColor,float blurDistance,float blurSampleLevel)
{
    float4 col1 = tex2Dlod(mainTex, float4(uv.x - blurDistance, uv.y + blurDistance, 0, blurSampleLevel));
    float4 col2 = tex2Dlod(mainTex, float4(uv.x, uv.y + blurDistance, 0, blurSampleLevel));
    float4 col3 = tex2Dlod(mainTex, float4(uv.x + blurDistance, uv.y + blurDistance, 0, blurSampleLevel));
    float4 col4 = tex2Dlod(mainTex, float4(uv.x - blurDistance, uv.y, 0, blurSampleLevel));
    float4 col5 = tex2Dlod(mainTex, float4(uv.x, uv.y, 0, blurSampleLevel));
    float4 col6 = tex2Dlod(mainTex, float4(uv.x + blurDistance, uv.y, 0, blurSampleLevel));
    float4 col7 = tex2Dlod(mainTex, float4(uv.x - blurDistance, uv.y - blurDistance, 0, blurSampleLevel));
    float4 col8 = tex2Dlod(mainTex, float4(uv.x, uv.y - blurDistance, 0, blurSampleLevel));
    float4 col9 = tex2Dlod(mainTex, float4(uv.x + blurDistance, uv.y - blurDistance, 0, blurSampleLevel));

    float4 finColor;
                
    if (blurKernel[4] != 0) 
    {
        finColor = (col1* blurKernel[0] + col2 * blurKernel[1] + col3 * blurKernel[2] +
            col4* blurKernel[3] + col5 * blurKernel[4] + col6 * blurKernel[5] +
            col7* blurKernel[6] + col8 * blurKernel[7] + col9* blurKernel[8]) * inColor;
    }
    else 
    {
        finColor = (col1* GaussianKernel[0] + col2 * GaussianKernel[1] + col3 * GaussianKernel[2] +
            col4* GaussianKernel[3] + col5 * GaussianKernel[4] + col6 * GaussianKernel[5] +
            col7* GaussianKernel[6] + col8 * GaussianKernel[7] + col9* GaussianKernel[8]) * inColor;
    }
    
    return finColor;
}


fixed4 GaussianBlurApplyChannel(sampler2D mainTex,float2 uv,float4 inColor,float blurDistance,float4 applyChannel)
{
    // sample texture an blur
    float4 col1 = tex2D(mainTex, float2(uv.x - blurDistance, uv.y + blurDistance));
    float4 col2 = tex2D(mainTex, float2(uv.x, uv.y + blurDistance));
    float4 col3 = tex2D(mainTex, float2(uv.x + blurDistance, uv.y + blurDistance)) ;
    float4 col4 = tex2D(mainTex, float2(uv.x - blurDistance, uv.y));
    float4 col5 = tex2D(mainTex, uv);
    float4 col6 = tex2D(mainTex, float2(uv.x + blurDistance, uv.y));
    float4 col7 = tex2D(mainTex, float2(uv.x - blurDistance, uv.y - blurDistance));
    float4 col8 = tex2D(mainTex, float2(uv.x, uv.y - blurDistance));
    float4 col9 = tex2D(mainTex, float2(uv.x + blurDistance, uv.y - blurDistance));

    float4 finColor;
                
    if (blurKernel[4] != 0) 
    {
        finColor = lerp(col5, (col1* blurKernel[0] + col2 * blurKernel[1] + col3 * blurKernel[2] +
            col4* blurKernel[3] + col5 * blurKernel[4] + col6 * blurKernel[5] +
            col7* blurKernel[6] + col8 * blurKernel[7] + col9* blurKernel[8]), applyChannel) * inColor;
    }
    else 
    {
        finColor = lerp(col5, (col1* GaussianKernel[0] + col2 * GaussianKernel[1] + col3 * GaussianKernel[2] +
            col4* GaussianKernel[3] + col5 * GaussianKernel[4] + col6 * GaussianKernel[5] +
            col7* GaussianKernel[6] + col8 * GaussianKernel[7] + col9* GaussianKernel[8]), applyChannel) * inColor;
    }

     return finColor;
}