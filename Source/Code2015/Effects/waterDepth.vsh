#define PlanetRadius 6371

#define waterBegin 20
#define waterEnd -15


float GetWaterDepth(float3 wpos)
{
    float height = length(wpos) - PlanetRadius;
    
    if (height < waterBegin)
    {
		return saturate((waterEnd-(height-waterBegin)) / (waterBegin-waterEnd));
    }
    return 0;
}
