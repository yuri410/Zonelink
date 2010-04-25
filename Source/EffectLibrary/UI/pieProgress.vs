struct VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
	float3 psPosition : TEXCOORD1;
};

VSOutput main(VSInput ip)
{
    VSOutput o;
    o.Position = ip.Position;
    o.TexCoord = ip.TexCoord;
    o.psPosition = o.Position.xyz;
    return o;
}
