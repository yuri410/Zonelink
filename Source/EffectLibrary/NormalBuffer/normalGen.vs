
float4x4 mvp;
float4x4 world;

void main(
   float4 pos : POSITION,
   float3 n : NORMAL,   
   out float4 oPos : POSITION,
   out float3 oN : TEXCOORD0)
{
    oPos = mul(pos, mvp);
    oN = mul(n, (float3x3)world);
}
