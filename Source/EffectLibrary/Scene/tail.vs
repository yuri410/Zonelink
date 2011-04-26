
float4x4 mvp : register(c0);
//float3 cameraZ : register(c4);

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0; 
    //float3 Direction : TEXCOORD1;
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};



VertexShaderOutput main(VertexShaderInput input)
{
    VertexShaderOutput output;
    //float3 tangent = cross(input.Direction, cameraZ);
	//t//angent = normalize(tangent);

	//tangent *= 500;

	//if (input.TexCoord.x<0.5)
	//{
	//	input.Position.xyz -= tangent;
	//}
	//else
	//{
	//	input.Position.xyz += tangent;
	//}
	output.Position = mul(mvp, input.Position);
	
	output.TexCoord = input.TexCoord;
    return output;
}

