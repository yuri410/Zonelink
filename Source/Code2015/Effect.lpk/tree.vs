#include "waterDepth.vsh"

float4x4 mvp : register(c0);

float4x4 world : register(c4);
float3 viewPos : register(c8);
float2 isVeg_wind : register(c9);

struct VSInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL;
    float3 TexCoord : TEXCOORD0;
    
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 ViewDir : TEXCOORD5;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

	float wind = 2 * 3.1415926 * (isVeg_wind.y + ip.TexCoord.z);
	
	ip.Position.x += 0.03 * sin(wind) * ip.Position.y;
	
    o.Position = mul(ip.Position, mvp);
	

    o.TexCoord = ip.TexCoord.xy;
    if (isVeg_wind.x>10)
	{
		ip.Normal = float3(0,1,0);
	}
	o.Normal = normalize((float3)mul(float4(ip.Normal,0), world));
    
    float3 wpos = mul(ip.Position, world).xyz;
    
	o.ViewDir = normalize(wpos - viewPos);
    return o;
}
