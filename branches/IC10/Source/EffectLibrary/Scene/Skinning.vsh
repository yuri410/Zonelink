

#define SKINNED_EFFECT_MAX_BONES   62

float4x3 Bones[SKINNED_EFFECT_MAX_BONES] : register(c14);



struct VSInputNmTxWeights
{
    float4 Position : Position;
    float3 Normal   : NORMAL;
    float2 TexCoord : TEXCOORD0;
    int4   Indices  : BLENDINDICES0;
    float4 Weights  : BLENDWEIGHT0;
};


void Skin(inout VSInputNmTxWeights vin, uniform int boneCount)
{
    float4x3 skinning = 0;

    [unroll]
    for (int i = 0; i < boneCount; i++)
    {
        skinning += Bones[vin.Indices[i]] * vin.Weights[i];
    }

    vin.Position.xyz = mul(vin.Position, skinning);
    vin.Normal = mul(vin.Normal, (float3x3)skinning);
}
