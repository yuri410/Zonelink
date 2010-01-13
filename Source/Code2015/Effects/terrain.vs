#include "waterDepth.vsh"

float4x4 mvp : register(c0);
//float4x4 invWorld : register(c4);
float terrSize : register(c8);
float4x4 world : register(c9);

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
    
	float3 TangentSpaceLDir : TEXCOORD6;
    float WaterDepth : TEXCOORD7;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    
    float4 wpos = mul(ip.Position, world);
    
	o.WaterDepth = GetWaterDepth((float3)mul(ip.Position, world));
    
    o.GlobeCoord = ip.GlobeCoord;
    
    o.DetailCoord.y = trunc(ip.Index/terrSize);
    o.DetailCoord.x = fmod(ip.Index, terrSize);
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
        
	float3 lightDir = float3(1,0,0);
    o.TangentSpaceLDir = (float3)mul(float4(lightDir,0), tanTrans);
    
    return o;
}
