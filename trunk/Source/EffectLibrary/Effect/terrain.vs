#include "waterDepth.vsh"

float4x4 mvp : register(c0);
float4x4 smTrans : register(c4);
float terrSize : register(c8);
float4x4 world : register(c9);
float3 viewPos : register(c14);
float3 lightDir : register(c15);

struct VSInput
{
    float4 Position : POSITION0;
    float2 GlobeCoord : TEXCOORD0; 
    float Index : TEXCOORD1;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 GlobeCoord : TEXCOORD0;
    float2 DetailCoord : TEXCOORD1;
    float4 smLgtPos : TEXCOORD2;
    float2 Depth : TEXCOORD3;
    float3 TangentSpaceVDir : TEXCOORD5;
	float3 TangentSpaceLDir : TEXCOORD6;
    float2 Height_Blend : TEXCOORD7;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    
    float4 wpos = mul(ip.Position, world);
    
	o.Height_Blend.x = GetHeight((float3)mul(ip.Position, world));
	o.Height_Blend.y = clamp( distance(viewPos, (float3)wpos ) / 2500 ,0.4,0.6);
    
    o.GlobeCoord = ip.GlobeCoord;
    
    
    
    o.DetailCoord.y = trunc((ip.Index+0.5) / (terrSize));
    o.DetailCoord.x = fmod(ip.Index+0.5, terrSize);
    o.DetailCoord /= terrSize;
    
    float3 normal = (float3)ip.Position;
	normal = normalize(normal);
	
	float3 biNormal = cross(float3(0,1,0), normal);
	biNormal = normalize(biNormal);
	float3 tangent = cross(normal, biNormal);
    
    float4x4 tanTrans;
    tanTrans[0] = float4(tangent, 0);
    tanTrans[1] = float4(biNormal, 0);
    tanTrans[2] = float4(normal, 0);
    tanTrans[3] = float4(0,0,0,1);
    tanTrans = transpose(tanTrans);

    o.TangentSpaceLDir = (float3)mul(float4(lightDir,0), tanTrans);
	o.TangentSpaceVDir = (float3)mul(float4(normalize(wpos.xyz - viewPos),0), tanTrans);

	o.Depth = o.Position.zw;
    o.smLgtPos = mul(ip.Position , smTrans);

    return o;
}
