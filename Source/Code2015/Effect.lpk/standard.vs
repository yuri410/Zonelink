#include "waterDepth.vsh"

float4x4 mvp : register(c0);

float4x4 world : register(c4);
float3 viewPos : register(c8);

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
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    o.TexCoord = ip.TexCoord;
    o.Normal = (float3)mul(float4(ip.Normal,0), world);

    return o;
}
