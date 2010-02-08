#define PlanetRadius 6371

#define waterBegin 1
#define waterEnd -18

float GetWaterDepth(float3 wpos)
{
    float height = length(wpos) - PlanetRadius;
    
    if (height < waterBegin)
    {
		float v = saturate((waterEnd-(height-waterBegin)) / (waterBegin-waterEnd));
		return sqrt(v);
    }
    return 0;
}
