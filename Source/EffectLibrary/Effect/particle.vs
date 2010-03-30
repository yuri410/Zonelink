#include "waterDepth.vsh"

float4x4 mvp : register(c0);
float size : register(c4);

struct VSInput
{
    float4 Position : POSITION0;
    float2 AlphaIdx : TEXCOORD0;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float Alpha : TEXCOORD0;
    float2 TexCoord1 : TEXCOORD1;
};

VSOutput main(VSInput ip)
{
	const float3 up = float3(0,1,0);
	const float3 right = float3(1,0,0);
    VSOutput o;

	float idx = ip.AlphaIdx.y;
	
	float ss = size;
	
	if (idx == 0)
	{
		ip.Position.xyz += (up + right) * -ss;
		o.TexCoord1 = float2(0, 1);
	}
	else if (idx == 1)
	{
		ip.Position.xyz += (right - up) * ss;
		o.TexCoord1 = float2(1, 1);
	}
	else if (idx == 2)
	{
		ip.Position.xyz += (up + right) * ss;
		o.TexCoord1 = float2(1, 0);
	}
	else
	{
		ip.Position.xyz += (right - up) * -ss;
		o.TexCoord1 = float2(0, 0);
	}
	
    o.Position = mul(ip.Position, mvp);
	o.Alpha = ip.AlphaIdx.x;

    return o;
}
