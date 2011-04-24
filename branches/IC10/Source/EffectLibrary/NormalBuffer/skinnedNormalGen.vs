
#define SKINNED_EFFECT_MAX_BONES   60

struct VSInputNmTxWeights
{
    float4 Position : Position;
    float3 Normal   : NORMAL;
    float2 TexCoord : TEXCOORD0;
    int4   Indices  : BLENDINDICES0;
    float4 Weights  : BLENDWEIGHT0;
};


float4x4 mvp : register(c0);
float4x4 worldView : register(c4);
float4x3 Bones[SKINNED_EFFECT_MAX_BONES] : register(c8);

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

void main(
   VSInputNmTxWeights vin,  
   out float4 oPos : POSITION,
   out float3 oN : TEXCOORD0,
   out float3 oTex : TEXCOORD1)
{
	Skin(vin);
	
    oPos = mul(vin.Position, mvp);
    oN = normalize(mul(vin.Normal, (float3x3)worldView));
    oTex.xy = vin.TexCoord;
    oTex.z = saturate(oPos.z/6500);
}
