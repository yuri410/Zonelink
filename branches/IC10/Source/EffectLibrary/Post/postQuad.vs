

struct VSInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

VSOutput main(VSInput ip)
{
	VSOutput o;
	o.Position = ip.Position;
	
	o.Position.xy -= 0.5;
	o.Position.xy /= float2(1024, 768);
	o.Position.xy *= float2(2, -2);
	o.Position.xy -= float2(1, -1);

	o.TexCoord = ip.TexCoord;
	return o;
}
