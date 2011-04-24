
float4x4 mvp;

struct VSInput
{
    float4 Position : POSITION0;
    float2 GlobeCoord : TEXCOORD0;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float3 GlobeCoord : TEXCOORD0;
};

VSOutput main(VSInput ip)
{
    VSOutput vo;
    
    vo.Position = mul(ip.Position, mvp);
    
    vo.GlobeCoord.xy = ip.GlobeCoord;
    vo.GlobeCoord.z = saturate(vo.Position.z/6500);
    
    return vo;
}
