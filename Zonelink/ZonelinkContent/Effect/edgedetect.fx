// This shader applies a Sobel filter to detect edges in the image.
// The Sobel filter extracts the first order derivates of the image,
// that is, the slope. Where the slope is sharp there is an edge.
// These are the filter kernels:
//
//  SobelX       SobelY
//  1  0 -1      1  2  1
//  2  0 -2      0  0  0
//  1  0 -1     -1 -2 -1


void VS_Main(float4 position: POSITION,
			 float2 texCoord: TEXCOORD0,
          out float4 outPosition:  POSITION£¬
          out float4 outTexCoord1: TEXCOORD0,
          out float4 outTexCoord2: TEXCOORD1,
          out float4 outTexCoord3: TEXCOORD2,
          out float4 outTexCoord4: TEXCOORD3,
          
          uniform float4 normalBufferSize)
{
  outPosition = position;

  // one pixel offset
  float off = 1.0 / normalBufferSize.x;

  // calculate all texture coords in the vertex shader to avoid pixel shader cost
  outTexCoord1 = float4(texCoord.x - off, texCoord.y - off,
                        texCoord.x, texCoord.y - off);
  outTexCoord2 = float4(texCoord.x + off, texCoord.y - off,
                        texCoord.x - off, texCoord.y);
  outTexCoord3 = float4(texCoord.x + off, texCoord.y,
                        texCoord.x - off, texCoord.y + off);
  outTexCoord4 = float4(texCoord.x, texCoord.y + off,
                        texCoord.x + off, texCoord.y + off);
}


texture normalBuffer;

sampler2D samNormalBuffer = sampler_state
{
    Texture = normalBuffer;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};



float4 PS_Main(float4 texCoord1: TEXCOORD0,
            float4 texCoord2: TEXCOORD1,
            float4 texCoord3: TEXCOORD2,
            float4 texCoord4: TEXCOORD3 ) : COLOR   
				
{
  // sample neighbor normals
  float3 s[3][3];

  s[0][0] = tex2D(samNormalBuffer, texCoord1.xy).rgb;
  s[0][1] = tex2D(samNormalBuffer, texCoord1.zw).rgb;
  s[0][2] = tex2D(samNormalBuffer, texCoord2.xy).rgb;
  s[1][0] = tex2D(samNormalBuffer, texCoord2.zw).rgb;
  s[1][2] = tex2D(samNormalBuffer, texCoord3.xy).rgb;
  s[2][0] = tex2D(samNormalBuffer, texCoord3.zw).rgb;
  s[2][1] = tex2D(samNormalBuffer, texCoord4.xy).rgb;
  s[2][2] = tex2D(samNormalBuffer, texCoord4.zw).rgb;

  // Sobel filter in X direction
  float3 sobelX = s[0][0] + 2 * s[1][0] + s[2][0] - s[0][2] - 2 * s[1][2] - s[2][2];
  // Sobel filter in Y direction
  float3 sobelY = s[0][0] + 2 * s[0][1] + s[0][2] - s[2][0] - 2 * s[2][1] - s[2][2];

  // Find edge
  float3 edgeSqr = (sobelX * sobelX + sobelY * sobelY);
  float value = dot(edgeSqr, float3(1));
  return float4(0, 0, 0, value * 0.1);
}


technique10 EdgeDetectTech
{
    pass P0
    {
        SetVertexShader( CompileShader( vs_4_0, VS_Main() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PS_Main() ) );
    }
}

