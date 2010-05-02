#include "waterDepth.vsh"

float4x4 mvp : register(c0);
float4x4 smTrans : register(c4);

float4x4 world : register(c9);
float3 lightDir : register(c13);

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
    
    float4 smLgtPos : TEXCOORD1;
    float2 Depth : TEXCOORD2;
    float3 TangentSpaceLDir : TEXCOORD3;
    float2 Height_Blend : TEXCOORD4;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    
    float4 wpos = mul(ip.Position, world);
    
	o.Height_Blend.x = GetHeight(wpos.xyz);
	o.Height_Blend.y = 0.5;
	
    o.GlobeCoord = ip.GlobeCoord;
    
    
    
    float3 normal = wpos.xyz;
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

	o.Depth.xy = o.Position.zw;
    o.smLgtPos = mul(ip.Position , smTrans);

    return o;
}
