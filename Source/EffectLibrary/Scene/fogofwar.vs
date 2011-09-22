#include "fog.vsh"

float4x4 mvp : register(c0);

struct VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float3 TexCoord : TEXCOORD0;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    o.TexCoord.xy = ip.TexCoord;
	o.TexCoord.z = GetFogFade(o.Position.z); // fog factor;
    return o;
}
