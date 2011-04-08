//#pragma profileoption PosInv 
//#pragma profileoption Posinv only works in the vertex shader 
//and causes the vertex shader to automatically match the fixed-function
//transform for the vertex position. There is no fixed-function geometry pipeline, 
//so its inapplicable there 

float4x4 modelMatrix;
float4x4 modelViewMatrix;
float4x4 shadowMatrix;
float2 range;

texture shadowMap;
sampler2D samShadowMap = sampler_state
{
    Texture = shadowMap;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};
  

void VS_Main(float4 position: POSITION,
          float3 normal: NORMAL,

          out float4 viewSpaceNormal: TEXCOORD0,
          out float3 worldSpaceNormal: TEXCOORD1,
          out float4 shadowCoord: TEXCOORD2)
{
  float4 viewSpacePosition = mul(modelViewMatrix, position);
  float depth = -viewSpacePosition.z;
  float fadeNear = range.x;
  float fadeFar = range.y;
  float alpha = clamp((depth - fadeNear) / (fadeFar - fadeNear), 0, 1);

  shadowCoord = mul(shadowMatrix, viewSpacePosition);

  viewSpaceNormal = float4(mul(float3x3(modelViewMatrix), normal), alpha);
  worldSpaceNormal = mul(float3x3(modelMatrix), normal);
}


float4 PS_Main(float4 viewSpaceNormal: TEXCOORD0,
              float3 worldSpaceNormal: TEXCOORD1,
              float4 shadowCoord: TEXCOORD2) : COLOR
{
	float3 N = normalize(worldSpaceNormal);
    float3 L = normalize(float3(1000, 1000, 1000));
    float diffuse = dot(N, L);

    float alpha = viewSpaceNormal.w;
    float3 n = lerp(normalize(viewSpaceNormal.xyz), float3(0, 0, 1), alpha);
    n = normalize(n);
    float shadow = tex2Dproj(shadowMap, shadowCoord).w;

    // put normal into rgb, shadow into alpha
    return float4(n * 0.5 + float3(0.5), shadow);
}


technique PrepassTech
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS_Main();
        PixelShader = compile ps_3_0 PS_Main();
    }
}
