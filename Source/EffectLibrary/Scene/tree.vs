#include "waterDepth.vsh"

float4x4 mvp : register(c0);

float4x4 world : register(c4);
float3 viewPos : register(c8);
float2 isVeg_wind : register(c9);
float4x4 smTrans : register(c13);

struct VSInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL;
    float3 TexCoord : TEXCOORD0;
    
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float4 smLgtPos : TEXCOORD2;
    float3 ViewDir : TEXCOORD5;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

	float wind = 2 * 3.1415926 * (isVeg_wind.y + ip.TexCoord.z);
	
	float4 xas = float4(1,0,0,0);
	float4 yas = float4(0,1,0,0);
	
	xas = mul(xas, world);
	yas = mul(yas, world);
	
	float3 pn = normalize(ip.Position.xyz);
	pn = ip.Position.xyz - pn * 6371;
	
	ip.Position += xas * (0.002 * sin(wind) * max(0, dot(yas.xyz,pn)));
	
    o.Position = mul(ip.Position, mvp);
	

    o.TexCoord = ip.TexCoord.xy;
    if (isVeg_wind.x>10)
	{
		o.Normal = yas.xyz;
	}
	else
	{
		o.Normal = normalize((float3)mul(float4(ip.Normal,0), world));
    }
    
    float3 wpos = ip.Position.xyz;
    
	o.ViewDir = normalize(wpos - viewPos);
	
	o.smLgtPos = mul(ip.Position , smTrans);
   return o;
}
