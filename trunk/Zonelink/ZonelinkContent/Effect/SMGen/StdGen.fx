
float4x4 mvp;

void VSMain(
   float4 pos : POSITION,
   out float4 oPos : POSITION, 
   out float depth:TEXCOORD0)
{
    oPos = mul(pos, mvp);
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
        VertexShader = compile ps_3_0 VSMain();
        PixelShader = compile ps_3_0 PSMain();
    }
}
