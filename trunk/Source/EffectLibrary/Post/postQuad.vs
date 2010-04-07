

struct VSInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
}

VSOutput main(VSInput ip) : POSITION0
{
	VSOutput o;
	o.Position = ip.Position;
	o.TexCoord = ip.TexCoord;
	return o;
}
