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
    o.Position.xy -= 0.5f;
    
    o.Position.xy /= float2(1280, 720);
    o.Position.y = 1 -o.Position.y;
    o.Position.xy -= 0.5f;
    o.Position.xy *=2;
    
    o.Position.z = 0;
    o.Position.w = 1;
    
    o.TexCoord = ip.TexCoord;
    o.psPosition = ip.Position.xyz;
    o.psPosition.xy -= float2(996, 18);
    return o;
}
