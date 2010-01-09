
float4x4 mvp : register(c0);
float4x4 invWorld : register(c4);
float terrSize : register(c8);
float4x4 world : register(c9);

struct VSInput
{
    float4 Position : POSITION0;
    float2 GlobeCoord : TEXCOORD0; 
    float Index : TEXCOORD1;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 GlobeCoord : TEXCOORD0;
    float2 DetailCoord : TEXCOORD1;
    
    float3 ModNormal : TEXCOORD2;
    
    float3 LightDir : TEXCOORD3;
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
        
    o.GlobeCoord = ip.GlobeCoord;
    
    o.DetailCoord.y = trunc(ip.Index/terrSize);
    o.DetailCoord.x = fmod(ip.Index, terrSize);
    o.DetailCoord /= terrSize;

	float3 center = (float3)mul(float4(0, 0, 0, 1), invWorld);
	o.ModNormal = (float3)ip.Position - center;
	
	//o.ModNormal.y *=0.67;
	o.ModNormal = normalize(o.ModNormal);
	
	o.LightDir = (float3)mul(float4(0.707, 0.707, 0, 0), invWorld);
    return o;
}
