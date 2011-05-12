#include "waterDepth.vsh"
#include "fog.vsh"

#define SKINNED_EFFECT_MAX_BONES   60



float4x4 mvp : register(c0);

float4x4 world : register(c4);
float3 viewPos : register(c8);
float4 isVegetation : register(c9);
float4x4 smTrans : register(c10);

float4x3 Bones[SKINNED_EFFECT_MAX_BONES] : register(c14);

struct VSInputNmTxWeights
{
    float4 Position : Position;
    float3 Normal   : NORMAL;
    float2 TexCoord : TEXCOORD0;
    int4   Indices  : BLENDINDICES0;
    float4 Weights  : BLENDWEIGHT0;
};


void Skin(inout VSInputNmTxWeights vin)
{
    float4x3 skinning = 0;

    [unroll]
    for (int i = 0; i < 4; i++)
    {
        skinning += Bones[vin.Indices[i]] * vin.Weights[i];
    }

    vin.Position.xyz = mul(vin.Position, skinning).xyz;
    vin.Normal = mul(vin.Normal, (float3x3)skinning);
}

struct VSOutput
{
    float4 Position : POSITION0;
    float3 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float4 smLgtPos : TEXCOORD2;
    float Depth : TEXCOORD3;
    float3 ViewDir : TEXCOORD5;
};

VSOutput main(VSInputNmTxWeights ip)
{
    VSOutput o;

    Skin(ip);
    
    o.Position = mul(ip.Position, mvp);
    o.TexCoord.xy = ip.TexCoord;
    o.TexCoord.z = GetFogFade(o.Position.z);
    if (isVegetation.x>10)
	{
		ip.Normal = float3(0,1,0);
	}
	
	o.Depth = o.Position.z;

    o.smLgtPos = mul(ip.Position , smTrans);
	o.Normal = normalize((float3)mul(float4(ip.Normal,0), world));
    
    float3 wpos = mul(ip.Position, world).xyz;
    
	o.ViewDir = normalize(wpos - viewPos);
    return o;
}
