#define SFUMATO_PERCENTAGE 90.0

#define FadeFogNear 3200.0
#define FadeFogFar 6000.0


float GetFogFade(float depth)
{
    float fogFade = clamp((depth - FadeFogNear) / (FadeFogFar - FadeFogNear), 0, SFUMATO_PERCENTAGE / 100.0);
    return fogFade;
}