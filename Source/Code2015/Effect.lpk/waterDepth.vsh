#define PlanetRadius 6371

float GetHeight(float3 wpos)
{
    return length(wpos) - PlanetRadius;
}