
float4x4 mvp;

void main(
   float4 pos : POSITION,
   out float4 oPos : POSITION, 
   out float2 depth:TEXCOORD0)
{
    oPos = mul(pos, mvp);
    depth.xy = oPos.zw;    
}
