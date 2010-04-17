float4x4 mvp : register(c0);

struct VSInput
{
    float4 Position : POSITION0;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 Depth : TEXCOORD0;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
	o.Depth = o.Position.zw;
	
    return o;
}
