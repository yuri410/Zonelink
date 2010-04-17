float4x4 mvp : register(c0);
float4x4 world : register(c4);
float3 viewPos : register(c8);
float3 lightDir : register(c9);
float4x4 smTrans : register(c13);

struct VSInput
{
    float4 Position : POSITION0;
    float2 NormalCoord : TEXCOORD0;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 NormalCoord : TEXCOORD0;
    float4 smLgtPos : TEXCOORD1;
    
    float4 Position2 : TEXCOORD4;
    float3 TangentSpaceVDir : TEXCOORD5;
    float3 TangentSpaceLDir : TEXCOORD6;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    o.NormalCoord = ip.NormalCoord * 5;
    o.Position2 = o.Position;
    
    float3 wpos = (float3)mul(ip.Position, world);
	float3 normal = normalize(wpos);
	
	
	float3 biNormal = cross(float3(0,1,0), normal);
	biNormal = normalize(biNormal);
	float3 tangent = cross(normal, biNormal);

	float4x4 tanTrans;
    tanTrans[0] = float4(tangent, 0);
    tanTrans[1] = float4(biNormal, 0);
    tanTrans[2] = float4(normal, 0);
    tanTrans[3] = float4(0,0,0,1);
    tanTrans = transpose(tanTrans);
    
    
    o.TangentSpaceVDir = (float3)mul(float4(normalize(wpos-viewPos),0), tanTrans);
    o.TangentSpaceLDir = (float3)mul(float4(lightDir,0), tanTrans);
    
    o.smLgtPos = mul(ip.Position , smTrans);

    return o;
}
