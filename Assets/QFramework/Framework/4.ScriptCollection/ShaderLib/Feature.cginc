// Create  by JiepengTan@gmail.com
// https://github.com/JiepengTan/FishManShaderTutorial
// 2018-03-27
#ifndef FMST_FEATURE
#define FMST_FEATURE

#include "Common.cginc"
#include "Noise.cginc"
#include "FBM.cginc"

float CausticRotateMin(float2 uv, float time){
	float3x3 mat = float3x3(2,1,-2, 3,-2,1, 1,2,2);
	float3 vec1 = mul(mat*0.5,float3(uv,time));
	float3 vec2 = mul(mat*0.4,vec1);
	float3 vec3 = mul(mat*0.3,vec2);
	float val = min(length(frac(vec1)-0.5),length(frac(vec2)-0.5));
	val = min(val,length(frac(vec3)-0.5));
	val = pow(val,7.0)*25.;
	return val;
}

float3 CausticTriTwist(float2 uv,float time )
{
	const int MAX_ITER = 5;
	float2 p = fmod(uv*PI2,PI2 )-250.0;

	float2 i = float2(p);
	float c = 1.0;
	float inten = .005;

	for (int n = 0; n < MAX_ITER; n++) 
	{
		float t = time * (1.0 - (3.5 / float(n+1)));
		i = p + float2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
		c += 1.0/length(float2(p.x / (sin(i.x+t)/inten),p.y / (cos(i.y+t)/inten)));
	}
    
	c /= float(MAX_ITER);
	c = 1.17-pow(c, 1.4);
	float val = pow(abs(c), 8.0);
	return val;
}

float CausticVoronoi(float2 p,float time) {
	float v = 0.0;
	float a = 0.4;
	for (int i = 0;i<3;i++) {
		v+= WNoise(p,time)*a;
		p*=2.0;
		a*=0.5;
	}
	v = pow(v,2.)*5.;
	return v;
}

float3 Stars(in float3 rd,float den,float tileNum)
{
    float3 c = float3(0.,0.,0.);
    float3 p = rd;
	float SIZE = 0.5;
    for (float i=0.;i<3.;i++)
    {
        float3 q = frac(p*tileNum)-0.5;
        float3 id = floor(p*tileNum);
        float2 rn = Hash33(id).xy;

		float size = (Hash13(id)*0.2+0.8)*SIZE; 
		float demp = pow(1.-size/SIZE,.8)*0.45;
		float val = (sin(_Time.y*31.*size)*demp+1.-demp) * size;
        float c2 = 1.-smoothstep(0.,val,length(q));
        c2 *= step(rn.x,(.0005+i*i*0.001)*den);
        c += c2*(lerp(float3(1.0,0.49,0.1),float3(0.75,0.9,1.),rn.y)*0.25+0.75);
        p *= 1.4;
    }
    return c*c*.7;
}


float TimeFBM( float2 p,float t )
{
    float2 f = 0.0;
	float s = 0.5;
	float sum =0;
	for(int i=0;i<5;i++){
		p += t;t *=1.5;
		f += s*tex2D(_NoiseTex, p/256).x; p = mul(float2x2(0.8,-0.6,0.6,0.8),p)*2.02;
		sum+= s;s*=0.6;
	}
    return f/sum;	 
}

float3 Cloud(float3 bgCol,float3 ro,float3 rd,float3 cloudCol,float spd, float layer)
{
	float3 col = bgCol;
    float time = _Time.y*0.05*spd;
	for(int i=0; i<layer; i++){
		float2 sc = ro.xz + rd.xz*((i+3)*40000.0-ro.y)/rd.y;
		col = lerp( col, cloudCol, 0.5*smoothstep(0.5,0.8,TimeFBM(0.00002*sc,time*(i+3))) );
	}
	return col;
}

fixed3 Fog(in fixed3 bgCol, in fixed3 ro, in fixed3 rd, in fixed maxT,
				float3 fogCol,float3 spd,float2 heightRange)
{
	fixed d = .4;
	float3 col = bgCol;
	for(int i=0; i<7; i++)
	{
		fixed3  p = ro + rd*d;
		// add some movement at some dir
		p += spd * ftime;
		p.z += sin(p.x*.5);
		// get height desity 
		float hDen = (1.-smoothstep(heightRange.x,heightRange.y,p.y));
		// get final  density
		fixed den = TNoise(p*2.2/(d+20.),ftime, 0.2)* hDen;
		fixed3 col2 = fogCol *( den *0.5+0.5);
		col = lerp(col,col2,clamp(den*smoothstep(d-0.4,d+2.+d*.75,maxT),0.,1.) );
		d *= 1.5+0.3; 
		if (d>maxT)break;
	}
	return col;
}

float3 Sky(float3 ro ,float3 rd,float3 lightDir){
	fixed3 col = fixed3(0.0,0.0,0.0);  
	float sundot = clamp(dot(rd,lightDir),0.0,1.0);
   
     // sky      
    col = float3(0.2,0.5,0.85)*1.1 - rd.y*rd.y*0.5;
    col = lerp( col, 0.85*float3(0.7,0.75,0.85), pow( 1.0-max(rd.y,0.0), 4.0 ) );
    // sun
    col += 0.25*float3(1.0,0.7,0.4)*pow( sundot,5.0 );
    col += 0.25*float3(1.0,0.8,0.6)*pow( sundot,64.0 );
    col += 0.4*float3(1.0,0.8,0.6)*pow( sundot,512.0 );
    // clouds
	col = Cloud(col,ro,rd,float3(1.0,0.95,1.0),1,1);
    // . 
    col = lerp( col, 0.68*float3(0.4,0.65,1.0), pow( 1.0-max(rd.y,0.0), 16.0 ) );
	return col;
}

// http://iquilezles.org/www/articles/checkerfiltering/checkerfiltering.htm
fixed CheckersGradBox( in fixed2 p )
{
    // filter kernel
    fixed2 w = fwidth(p) + 0.001;
    // analytical integral (box filter)
    fixed2 i = 2.0*(abs(frac((p-0.5*w)*0.5)-0.5)-abs(frac((p+0.5*w)*0.5)-0.5))/w;
    // xor pattern
    return 0.5 - 0.5*i.x*i.y;                  
}


float _Ripple(float period,float spreadSpd,float waveGap,float2 uv,float rnd){
	 // sample the texture
	const float WAVE_NUM = 2.;
    const float  CROSS_NUM = 1.0;
    float ww = -WAVE_NUM * .5 * waveGap;
    float hww = ww * 0.5;
    float freq = WAVE_NUM * PI2 / waveGap/(CROSS_NUM + 1.);
    float radius = (float(CROSS_NUM));
    float2 p0 = floor(uv);
    float sum = 0.;

    for (float j = -CROSS_NUM; j <= CROSS_NUM; ++j){
        for (float i = -CROSS_NUM; i <= CROSS_NUM; ++i){
            float2 pi = p0 + float2(i, j);
            float2 h22 = Hash23(float3(pi,rnd));
            float h12 = Hash13(float3(pi,rnd));
            float pd = period*( h12 * 1.+ 1.);
            float time = ftime+pd*h12;
            float t = fmod(time,pd);
			float spd = spreadSpd*((1.0-h12) * 0.2 + 0.8);
            float size = (h12)*0.4+0.6;
            float maxt = min(pd*0.6,radius *size /spd);
            float amp = clamp01(1.- t/maxt);
            float2 p = pi +  Hash21(h12 + floor(time/pd)) * 0.4;
            float d = (length(p - uv) - spd*t)/radius * 0.5;
            sum -= amp*sin(freq*d) *  smoothstep(ww*size, hww*size, d) *  smoothstep(0., hww*size, d);
        }
    }
    sum /= (CROSS_NUM*2+1)*(CROSS_NUM*2+1);
    return sum;
}

float Ripples(float2 uv ,float layerNum,float tileNum,float period,float spreadSpd,float waveGap){
	float sum = 0.;
	for(int i =0;i<layerNum;i++){
		sum += _Ripple(period,spreadSpd,waveGap,uv*(1.+i/layerNum ) * tileNum,float(i));
	}
	return sum ;
}

#define Caustic CausticRotateMin

#endif // FMST_NOISE