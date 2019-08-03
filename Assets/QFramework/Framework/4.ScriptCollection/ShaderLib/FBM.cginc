// Create  by JiepengTan@gmail.com
// https://github.com/JiepengTan/FishManShaderTutorial
// 2018-03-27
#ifndef FMST_FBM
#define FMST_FBM

#include "Noise.cginc"


float FBM( float2 p )
{
    float f = 0.0;
    f += 0.50000*Noise( p*1.0  ); 
    f += 0.25000*Noise( p*2.03  ); 
    f += 0.12500*Noise( p*4.01  ); 
    f += 0.06250*Noise( p*8.05  ); 
    f += 0.03125*Noise( p*16.02 );
    return f/0.984375;
}
float FBM( float2 p,float iterNum)
{
    float f = 0.0;
	float s = 0.5;
	float s2 = 2.00;
	float sum = 0.0;
	for(int i = 0;i< iterNum;i++){
		f += s*Noise( p ); 
		p *=s2;
		sum+=s;
		s*= 0.5;s2+=0.01;
	}
	return f/sum;
}

float FBMR( float2 p )
{
    float f = 0.0;

    f += 0.50000*Noise( p ); p = mul(_m2,p)*2.02;
    f += 0.25000*Noise( p ); p = mul(_m2,p)*2.03;
    f += 0.12500*Noise( p ); p = mul(_m2,p)*2.01;
    f += 0.06250*Noise( p ); p = mul(_m2,p)*2.04;
    f += 0.03125*Noise( p );
    return f/0.984375;
}
float FBMR( float3 p )
{
    float f = 0.0;

    f += 0.50000*Noise( p ); p = mul(_m3,p)*2.02;
    f += 0.25000*Noise( p ); p = mul(_m3,p)*2.03;
    f += 0.12500*Noise( p ); p = mul(_m3,p)*2.01;
    f += 0.06250*Noise( p ); p = mul(_m3,p)*2.04;
    f += 0.03125*Noise( p );
    return f/0.984375;
}
float FBMR( float2 p,float iterNum)
{
    float f = 0.0;
	float s = 0.5;
	float s2 = 2.00;
	float sum = 0.0;
	for(int i = 0;i< iterNum;i++){
		f += s*Noise( p ); 
		p = mul(_m2,p)*s2;
		sum+=s;
		s*= 0.5;s2+=0.01;
	}
	return f/sum;
}

float FBM( in float3 p )
{
    float n = 0.0;
    n += 0.50000*Noise( p*1.0 );
    n += 0.25000*Noise( p*2.0 );
    n += 0.12500*Noise( p*4.0 );
    n += 0.06250*Noise( p*8.0 );
    n += 0.03125*Noise( p*16.0 );
    return n/0.984375;
}

float FBM( float3 p,float iterNum)
{
    float f = 0.0;
	float s = 0.5;
	float s2 = 2.00;
	float sum = 0.0;
	for(int i = 0;i< iterNum;i++){
		f += s*Noise( p ); 
		p *=s2;
		sum+=s;
		s*= 0.5;s2+=0.01;
	}
	return f/sum;
}

#endif // FMST_FBM
