// Create  by JiepengTan@gmail.com
// https://github.com/JiepengTan/FishManShaderTutorial
// 2018-03-27
#ifndef FMST_MATH
#define FMST_MATH

#define PI 3.14159265359
#define PI2 6.28318530718
#define Deg2Radius PI/180.
#define Radius2Deg 180./PI

#define clamp01(a) clamp(a,0.0,1.0)

float length2( float2 p )
{
    return sqrt( p.x*p.x + p.y*p.y );
}

float length6( float2 p )
{
    p = p*p*p; p = p*p;
    return pow( p.x + p.y, 1.0/6.0 );
}

float length8( float2 p )
{
    p = p*p; p = p*p; p = p*p;
    return pow( p.x + p.y, 1.0/8.0 );
}


float smin( float a, float b, float k )
{
    float h = clamp( 0.5+0.5*(b-a)/k, 0.0, 1.0 );
    return lerp( b, a, h ) - k*h*(1.0-h);
}
#define _m2 (float2x2(0.8,-0.6,0.6,0.8))
#define _m3 (float3x3( 0.00,  0.80,  0.60, -0.80,  0.36, -0.48, -0.60, -0.48,  0.64 ))

float2x2 Rot2D(float a){a*= Radius2Deg; float sa = sin(a); float ca = cos(a); return float2x2(ca,-sa,sa,ca);}
float2x2 Rot2DRad(float a){float sa = sin(a); float ca = cos(a); return float2x2(ca,-sa,sa,ca);}


float3x3 Rotx(float a){a*= Radius2Deg; float sa = sin(a); float ca = cos(a); return float3x3(1.,.0,.0,    .0,ca,sa,   .0,-sa,ca);}
float3x3 Roty(float a){a*= Radius2Deg; float sa = sin(a); float ca = cos(a); return float3x3(ca,.0,sa,    .0,1.,.0,   -sa,.0,ca);}
float3x3 Rotz(float a){a*= Radius2Deg; float sa = sin(a); float ca = cos(a); return float3x3(ca,sa,.0,    -sa,ca,.0,  .0,.0,1.); }

float3x3 RotEuler(float3 ang) {
	ang = ang*Radius2Deg;
    float2 a1 = float2(sin(ang.x),cos(ang.x));
    float2 a2 = float2(sin(ang.y),cos(ang.y));
    float2 a3 = float2(sin(ang.z),cos(ang.z));
    float3x3 m;
    m[0] = float3(a1.y*a3.y+a1.x*a2.x*a3.x,a1.y*a2.x*a3.x+a3.y*a1.x,-a2.y*a3.x);
    m[1] = float3(-a2.y*a1.x,a1.y*a2.y,a2.x);
    m[2] = float3(a3.y*a1.x*a2.x+a1.y*a3.x,a1.x*a3.x-a1.y*a3.y*a2.x,a2.y*a3.y);
    return m;
}

float Remap(float oa,float ob,float na,float nb,float val){
	return (val-oa)/(ob-oa) * (nb-na) + na;
}
#endif // FMST_MATH