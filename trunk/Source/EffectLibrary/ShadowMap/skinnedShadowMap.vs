
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
float4x3 Bones[SKINNED_EFFECT_MAX_BONES] : register(c4);

void Skin(inout VSInputNmTxWeights vin)
{
    float4x3 skinning = 0;

    [unroll]
    for (int i = 0; i < 4; i++)
    {
        skinning += Bones[vin.Indices[i]] * vin.Weights[i];
    }

    vin.Position.xyz = mul(vin.Position, skinning).xyz;
}

void main(VSInputNmTxWeights vin,
   out float4 oPos : POSITION, 
   out float depth:TEXCOORD0)
{
	Skin(vin);
	
    oPos = mul(vin.Position, mvp);
    depth = oPos.z;    
}
