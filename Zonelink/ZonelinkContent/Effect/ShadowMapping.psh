#define SHADOW_EPSILON 0.001f
#define SMAP_SIZE 512


float VSM_FILTER(sampler2D sm, float2 tex, float fragDepth )
{
    float lit = 0.0f;
    float2 moments = tex2D(sm, tex).rg;
    
    float E_x2 = moments.y;
    float Ex_2 = moments.x * moments.x;
    float variance = E_x2 - Ex_2;    
    float mD = (moments.x - fragDepth );
    float mD_2 = mD * mD;
    float p = variance / (variance + mD_2 );
    lit = max( p, fragDepth <= moments.x );
    
    return min(lit+0.25, 1);
}