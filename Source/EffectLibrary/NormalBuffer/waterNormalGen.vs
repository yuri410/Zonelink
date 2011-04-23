float4x4 mvp : register(c0);

struct VSInput
{
    float4 Position : POSITION0;
    float3 NormalCoord : TEXCOORD0;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float3 NormalCoord : TEXCOORD0;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    o.NormalCoord = ip.NormalCoord * 3.5;
    o.NormalCoord.z = saturate(o.Position.z/6000);
    return o;
}
