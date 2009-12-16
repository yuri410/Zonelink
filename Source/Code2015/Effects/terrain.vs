float4x4 mvp : register(c0);

struct VSInput
{
    float3 Position : POSITION0;
    float2 TexCoord1 : TEXCOORD0; 
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord1 : TEXCOORD0; 
};

VSOutput main(VSInput ip)
{
    VSOutput o;
    o.Position = mul(ip.Position, mvp);
    o.TexCoord1 = ip.TexCoord1;
    return o;
}
