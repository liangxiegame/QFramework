// Create  by JiepengTan@gmail.com
// https://github.com/JiepengTan/FishManShaderTutorial
// 2018-03-27
#ifndef FMST_SDF
#define FMST_SDF
#include "Math.cginc"

//------------------------------------------------------------------

float OpS( float d1, float d2 )
{
    return max(-d2,d1);
}

float OpU( float d1, float d2 )
{
    return min(d1,d2);
}
float2 OpU( float2 d1, float2 d2 )
{
    return (d1.x<d2.x) ? d1 : d2;
}

float3 OpRep( float3 p, float3 c )
{
    return fmod(p,c)-0.5*c;
}

float2 OpRep( float2 p, float2 c )
{
    return fmod(p,c)-0.5*c;
}


float3 OpTwist( float3 p )
{
    float  c = cos(10.0*p.y+10.0);
    float  s = sin(10.0*p.y+10.0);
    float2x2   m = float2x2(c,-s,s,c);
    return float3(mul(m,p.xz),p.y);
}



float SdPlane( float3 p )
{
    return p.y;
}

float SdSphere( float3 p, float s )
{
    return length(p)-s;
}

float SdBox( float3 p, float3 b )
{
    float3 d = abs(p) - b;
    return min(max(d.x,max(d.y,d.z)),0.0) + length(max(d,0.0));
}

float SdEllipsoid( in float3 p, in float3 r )
{
    return (length( p/r ) - 1.0) * min(min(r.x,r.y),r.z);
}

float SdRoundBox( float3 p, float3 b, float r )
{
    return length(max(abs(p)-b,0.0))-r;
}

float SdTorus( float3 p, float2 t )
{
    return length( float2(length(p.xz)-t.x,p.y) )-t.y;
}

float SdHexPrism( float3 p, float2 h )
{
    float3 q = abs(p);
#if 0
    return max(q.z-h.y,max((q.x*0.866025+q.y*0.5),q.y)-h.x);
#else
    float d1 = q.z-h.y;
    float d2 = max((q.x*0.866025+q.y*0.5),q.y)-h.x;
    return length(max(float2(d1,d2),0.0)) + min(max(d1,d2), 0.);
#endif
}

float SdCapsule( float3 p, float3 a, float3 b, float r )
{
    float3 pa = p-a, ba = b-a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h ) - r;
}

float SdEquilateralTriangle(  in float2 p )
{
    const float k = 1.73205;//sqrt(3.0);
    p.x = abs(p.x) - 1.0;
    p.y = p.y + 1.0/k;
    if( p.x + k*p.y > 0.0 ) p = float2( p.x - k*p.y, -k*p.x - p.y )/2.0;
    p.x += 2.0 - 2.0*clamp( (p.x+2.0)/2.0, 0.0, 1.0 );
    return -length(p)*sign(p.y);
}

float SdTriPrism( float3 p, float2 h )
{
    float3 q = abs(p);
    float d1 = q.z-h.y;
#if 1
    // distance bound
    float d2 = max(q.x*0.866025+p.y*0.5,-p.y)-h.x*0.5;
#else
    // correct distance
    h.x *= 0.866025;
    float d2 = SdEquilateralTriangle(p.xy/h.x)*h.x;
#endif
    return length(max(float2(d1,d2),0.0)) + min(max(d1,d2), 0.);
}



float SdCylinder( float3 p, float2 h )
{
  float2 d = abs(float2(length(p.xz),p.y)) - h;
  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}

float SdCone( in float3 p, in float3 c )
{
    float2 q = float2( length(p.xz), p.y );
    float d1 = -q.y-c.z;
    float d2 = max( dot(q,c.xy), q.y);
    return length(max(float2(d1,d2),0.0)) + min(max(d1,d2), 0.);
}

float SdConeSection( in float3 p, in float h, in float r1, in float r2 )
{
    float d1 = -p.y - h;
    float q = p.y - h;
    float si = 0.5*(r1-r2)/h;
    float d2 = max( sqrt( dot(p.xz,p.xz)*(1.0-si*si)) + q*si - r2, q );
    return length(max(float2(d1,d2),0.0)) + min(max(d1,d2), 0.);
}

float SdPryamid4(float3 p, float3 h ) // h = { cos a, sin a, height }
{
    // Tetrahedron = Octahedron - Cube
    float box = SdBox( p - float3(0,-2.0*h.z,0), float3(2.0*h.z,2.0*h.z,2.0*h.z) );
 
    float d = 0.0;
    d = max( d, abs( dot(p, float3( -h.x, h.y, 0 )) ));
    d = max( d, abs( dot(p, float3(  h.x, h.y, 0 )) ));
    d = max( d, abs( dot(p, float3(  0, h.y, h.x )) ));
    d = max( d, abs( dot(p, float3(  0, h.y,-h.x )) ));
    float octa = d - h.z;
    return max(-box,octa); // Subtraction
 }


float SdTorus82( float3 p, float2 t )
{
    float2 q = float2(length2(p.xz)-t.x,p.y);
    return length8(q)-t.y;
}

float SdTorus88( float3 p, float2 t )
{
    float2 q = float2(length8(p.xz)-t.x,p.y);
    return length8(q)-t.y;
}

float SdCylinder6( float3 p, float2 h )
{
    return max( length6(p.xz)-h.x, abs(p.y)-h.y );
}

//圆柱切片
float SdClipCylinder( float3 pos, float3 h)
{
   pos.x += h.x*h.z*2. - h.x;
   float cy = SdCylinder(pos,h.xy);
   float bx = SdBox(pos-float3(h.x*(1.+2.*h.z),0.,0.),float3(h.x*2.,h.y+0.3,h.x*2.));
   return OpS(cy,bx);
}

#endif // FMST_SDF