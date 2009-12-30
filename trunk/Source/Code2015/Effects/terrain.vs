float4x4 mvp : register(c0);
float4x4 world;

struct VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord1 : TEXCOORD0; 
    float4 Normal : TEXCOORD1;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord1 : TEXCOORD0;
    float2 TexCoord2 : TEXCOORD1;
    float3 Normal : TEXCOORD2;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    o.TexCoord1 = ip.TexCoord1;
    o.TexCoord2 = float2( ip.Position.x / 20, ip.Position.z / 20);
    
    ip.Normal = 2 * (ip.Normal - float4(0.5,0.5,0.5, 1));
    ip.Normal.w = 0;
    
    o.Normal = (float3)mul(ip.Normal, world);
    //o.Normal = (float3) ip.Normal;
    o.Normal = normalize(o.Normal);
    return o;
}
