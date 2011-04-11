
float4x4 mvp;

struct VSInput
{
    float4 Position : POSITION0;
    float2 GlobeCoord : TEXCOORD0;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 GlobeCoord : TEXCOORD0;
};

VSOutput main(VSInput ip)
{
    VSOutput vo;
    
    vo.Position = mul(ip.Position, mvp);
    
    vo.GlobeCoord = ip.GlobeCoord;
    
    
    return vo;
}
