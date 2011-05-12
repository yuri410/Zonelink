#include "waterDepth.vsh"

float4x4 mvp : register(c0);


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
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    o.TexCoord = ip.TexCoord;
    
    float tmp = o.TexCoord.x;
    o.TexCoord.x = o.TexCoord.y;
    o.TexCoord.y = tmp;
	//o.TexCoord.x = 1- o.TexCoord.x;
	//o.TexCoord = 1- o.TexCoord;
    return o;
}
