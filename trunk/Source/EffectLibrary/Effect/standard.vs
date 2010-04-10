#include "waterDepth.vsh"

float4x4 mvp : register(c0);

float4x4 world : register(c4);
float3 viewPos : register(c8);
float4 isVegetation : register(c9);
float4x4 smTrans : register(c10);

struct VSInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL;
    float2 TexCoord : TEXCOORD0;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float4 smLgtPos : TEXCOORD2;
    float2 Depth : TEXCOORD3;
    float3 ViewDir : TEXCOORD5;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    o.TexCoord = ip.TexCoord;
    if (isVegetation.x>10)
	{
		ip.Normal = float3(0,1,0);
	}
	
	o.Depth = o.Position.zw;

    o.smLgtPos = mul(ip.Position , smTrans);
	o.Normal = normalize((float3)mul(float4(ip.Normal,0), world));
    
    float3 wpos = mul(ip.Position, world).xyz;
    
	o.ViewDir = normalize(wpos - viewPos);
    return o;
}
