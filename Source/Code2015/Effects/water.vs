float4x4 mvp : register(c0);

struct VSInput
{
    float4 Position : POSITION0;
    float2 NormalCoord : TEXCOORD0;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 NormalCoord : TEXCOORD0;
    float3 ModNormal : TEXCOORD1;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    o.NormalCoord = ip.NormalCoord * 5;
    
    o.ModNormal = ip.Position;//(float3)mul(ip.Position, world);

	o.ModNormal = normalize(o.ModNormal);

    
    return o;
}
