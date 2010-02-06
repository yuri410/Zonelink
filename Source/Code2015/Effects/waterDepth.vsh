#define PlanetRadius 6371

#define waterBegin 5
//#define waterEnd -14
#define waterEnd -17

float GetWaterDepth(float3 wpos)
{
    float height = length(wpos) - PlanetRadius-10;
    
    if (height < waterBegin)
    {
		return saturate((waterEnd-(height-waterBegin)) / (waterBegin-waterEnd));
    }
    return 0;
}
