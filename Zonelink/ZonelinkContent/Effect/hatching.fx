#include "api.cg"
#ifdef SCAPE_CG_OPENGL
#  pragma profileoption PosInv
#endif


float4x4 modelViewProjectionMatrix;
float4x4 modelViewMatrix;
float4x4 modelMatrix;
float4 cameraSettings;


const float SFUMATO_PERCENTAGE = 90;


void VS_Main(float4 position: POSITION,
          float2 texCoord: TEXCOORD0,
          float2 hatchTexCoord: TEXCOORD1,
          float3 normal: NORMAL,
//        float4 color: COLOR,

#ifndef SCAPE_CG_OPENGL
          out float4 outPosition: POSITION,
#endif
//          out float4 outColor: COLOR,
          out float4 outTexCoord: TEXCOORD0,
          out float4 shadowCoord: TEXCOORD1,
          out float3 outNormal: TEXCOORD2,
          out float2 outAlphaAndBeta: TEXCOORD3)
{
  float4 clipSpacePos = mul(modelViewProjectionMatrix, position);
  
#ifndef SCAPE_CG_OPENGL
  outPosition = clipSpacePos;
#endif

  clipSpacePos.xy = (clipSpacePos.xy + float2(clipSpacePos.w)) * 0.5;
  shadowCoord = clipSpacePos;

  float4 viewSpacePosition = mul(modelViewMatrix, position);

  float fadeFogNear = cameraSettings.x;
  float fadeFogFar = cameraSettings.y;
  float fadeHatchNear = cameraSettings.z;
  float fadeHatchFar = cameraSettings.w;
  
  float depth = length(viewSpacePosition.xyz);
  outAlphaAndBeta.x = clamp((depth - fadeHatchNear) / (fadeHatchFar - fadeHatchNear), 0, 1);
  outAlphaAndBeta.y = clamp((depth - fadeFogNear) / (fadeFogFar - fadeFogNear), 0, SFUMATO_PERCENTAGE / 100.0);

  outTexCoord = float4(texCoord.x, texCoord.y, hatchTexCoord.x, hatchTexCoord.y);

//  outColor = color;
  
  outNormal = mul(float3x3(modelMatrix), normal);
}


//----------------------------------------------------------------------------------------------------
float hatchLayers;
float4 blendColor;
float3 sfumatoColor;
float3 lightPosition;


texture diffuseMap;
sampler2D samDiffuseMap = sampler_state
{
    Texture = diffuseMap;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};

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

texture hatch0;
sampler2D samHatch0 = sampler_state
{
    Texture = shadowMap;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};


texture hatch1;
sampler2D samHatch1 = sampler_state
{
    Texture = shadowMap;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};
            
            


#include "assets/shaders/computeweights.cg"

const float3 SHADOW_COLOR = float3(39,40,47) / 255.0;
const float AMBIENT = 0.25;

//#define NEGATIVE_HATCH

float4 PS_Main(float4 texCoord: TEXCOORD0,
            float4 shadowCoord: TEXCOORD1,
            float3 normal: TEXCOORD2,
            float2 alphaAndBeta: TEXCOORD3,
//            float4 vertexColor: COLOR,
            ) : COLOR
{
	
    float3 N = normalize(normal);
    float3 L = normalize(lightPosition-shadowCoord.xyz);
    float diffuse = dot(N, L) * (1-AMBIENT) + AMBIENT;

    float shadow = tex2Dproj(shadowMap, shadowCoord).a;
    float ambientShadow = shadow * (1-AMBIENT) + AMBIENT;
    float shadowFade = alphaAndBeta.x;
    diffuse = lerp(ambientShadow*diffuse, diffuse, shadowFade);
    float4 diffuseColor = tex2D(diffuseMap, texCoord.xy);
    float ambient = diffuseColor.a;

    float hatchFactor = clamp((diffuse) * ambient, 0, 1) * (hatchLayers - 1) + 1;
    float3 weightsA = float3(5.0, 4.0, 3.0);
    
    float3 hatchWeightsA = computeWeights(hatchFactor, weightsA);
    float3 weightsB = float3(2.0, 1.0, 0.0);
    float3 hatchWeightsB = computeWeights(hatchFactor, weightsB);

#ifdef NEGATIVE_HATCH
    float3 h0 = tex2D(hatch0, texCoord.zw).rgb;
    float3 h1 = tex2D(hatch1, texCoord.zw).rgb;
    h1 = float3(h1.r, 1-h0.b, 1-h0.g) * hatchWeightsB;
    h0 = h0 * hatchWeightsA;
#else
    float3 h0 = tex2D(hatch0, texCoord.zw).rgb * hatchWeightsA;
    float3 h1 = tex2D(hatch1, texCoord.zw).rgb * hatchWeightsB;
#endif

    float hatch = dot(h0, float3(1)) + dot(h1, float3(1));

    float sfumatoFade = alphaAndBeta.y;
    float3 hatchColor = SHADOW_COLOR + float3(hatch);
    diffuseColor.rgb *= hatchColor;
    float3 color = lerp(diffuseColor.rgb, sfumatoColor, sfumatoFade); 
    return float4(color, 1);
}


technique10 HatchingTech
{
    pass P0
    {
        SetVertexShader( CompileShader( vs_4_0, VS_Main() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PS_Main() ) );
    }
}
