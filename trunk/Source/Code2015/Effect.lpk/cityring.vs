float4x4 mvp : register(c0);
float4x4 world : register(c4);

struct VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;

};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
	
	float3 psPosition : TEXCOORD2;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    o.TexCoord = ip.TexCoord;
	o.Normal = normalize((float3)mul(float4(ip.Normal,0), world));
	o.psPosition = ip.Position;
    return o;
}
