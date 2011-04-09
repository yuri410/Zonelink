#include "..\Skinning.fxh"

float4x4 mvp;

void VSMain(VSInputNmTxWeights vin,
   out float4 oPos : POSITION, 
   out float depth:TEXCOORD0)
{
    Skin(vin, 1);

    oPos = mul(vin.Position, mvp);
    depth = oPos.z;    
}

float4 PSMain( float depth : TEXCOORD0) : COLOR 
{
    return float4( depth, depth * depth, 0, 1);
}
technique StdGenSM
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSMain();
        PixelShader = compile ps_3_0 PSMain();
    }
}
