
float4x4 mvp;
float4x4 worldView;

void main(
   float4 pos : POSITION,
   float3 n : NORMAL, 
   float2 tex : TEXCOORD0,  
   out float4 oPos : POSITION,
   out float3 oN : TEXCOORD0,
   out float2 oTex : TEXCOORD1)
{
    oPos = mul(pos, mvp);
    oN = normalize(mul(n, (float3x3)worldView));
    oTex = tex;
}
