#define PlanetRadius 6371

#define seaLevel 2
#define waterEnd -20

float GetWaterDepth(float3 pos, float4x4 world)
{
    float4 wpos = mul(pos, world);
    
    float height = length(wpos) - PlanetRadius;
    
    if (height <seaLevel)
    {
		return height /(seaLevel-waterEnd);
    }
    return 0;
}   