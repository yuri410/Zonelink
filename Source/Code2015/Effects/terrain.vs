
float4x4 mvp : register(c0);
//float4x4 invWorld : register(c4);
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
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
        
    o.GlobeCoord = ip.GlobeCoord;
    
    o.DetailCoord.y = trunc(ip.Index/terrSize);
    o.DetailCoord.x = fmod(ip.Index, terrSize);
    o.DetailCoord /= terrSize;

	o.ModNormal = (float3)ip.Position;
	
	//o.ModNormal.y *=0.67;
	o.ModNormal = normalize(o.ModNormal);
	
    return o;
}
