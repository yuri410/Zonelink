

#define SAMPLE_COUNT 5

texture tex;
sampler2D samTex = sampler_state
{
    Texture = tex;
    MinFilter = POINT;
    MagFilter = POINT;    
    MipFilter = NONE;
	
	AddressU = CLAMP;
	AddressV = CLAMP;
};

float2 SampleOffsets[SAMPLE_COUNT] : register(c0);
float SampleWeights[SAMPLE_COUNT] : register(c15);

struct PSInput
{
	float2 TexCoord : TEXCOORD0;
};

struct PSOutput
{
    float4 Color : COLOR;
};

float4 PSMain(PSInput ip) : COLOR
{
	float4 color = 0;

	for (int i=0;i<SAMPLE_COUNT;i++)
		color += tex2D(samTex, ip.TexCoord + SampleOffsets[i])*SampleWeights[i];

    return color;
}

technique ShadowBlur
{
    pass P0
    {
        //VertexShader = NULL;
        PixelShader = compile ps_2_0 PSMain();
    }
}

