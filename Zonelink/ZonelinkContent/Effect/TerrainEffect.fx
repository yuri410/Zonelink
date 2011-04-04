
float4x4 mvp;
float4x4 smTrans;
float terrSize;
float4x4 world;
float3 lightDir;


struct VSInput
{
    float4 Position : POSITION0;
    float2 GlobeCoord : TEXCOORD0; 
    float Index : TEXCOORD1;
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 GlobeCoord : TEXCOORD0;
    float2 DetailCoord : TEXCOORD1;
    float4 smLgtPos : TEXCOORD2;
    float Depth : TEXCOORD3;
    float3 TangentSpaceLDir : TEXCOORD4;
    float2 Height_Blend : TEXCOORD5;
};

VSOutput VSMain(VSInput ip)
{
    VSOutput o;

    o.Position = mul(ip.Position, mvp);
    
    float4 wpos = mul(ip.Position, world);
    
	//o.Height_Blend.x = GetHeight(wpos.xyz);
	o.Height_Blend.x = 0;
	o.Height_Blend.y = 0.5;//clamp( distance(viewPos, (float3)wpos ) / 2500 ,0.4,0.6);
    
    o.GlobeCoord = ip.GlobeCoord;
    
    o.DetailCoord.y = trunc((ip.Index+0.5) / (terrSize));
    o.DetailCoord.x = fmod(ip.Index+0.5, terrSize);
    o.DetailCoord /= terrSize;
    
    float3 normal = (float3)ip.Position;
	normal = normalize(normal);
	
	float3 biNormal = cross(float3(0,1,0), normal);
	biNormal = normalize(biNormal);
	float3 tangent = cross(normal, biNormal);
    
    float4x4 tanTrans;
    tanTrans[0] = float4(tangent, 0);
    tanTrans[1] = float4(biNormal, 0);
    tanTrans[2] = float4(normal, 0);
    tanTrans[3] = float4(0,0,0,1);
    tanTrans = transpose(tanTrans);

    o.TangentSpaceLDir = (float3)mul(float4(lightDir,0), tanTrans);

	o.Depth = o.Position.z;
    o.smLgtPos = mul(ip.Position , smTrans);

    return o;
}





texture texDet1;
texture texDet2;
texture texDet3;
texture texDet4;
texture texShd;
texture texIdx;
texture texColor;
texture texNorm;
texture texCliff;


sampler2D samTexIndex = sampler_state
{
    Texture = texIdx;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};
sampler2D samTexDet1 = sampler_state
{
    Texture = texDet1;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};
sampler2D samTexDet2  = sampler_state
{
    Texture = texDet2;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};
sampler2D samTexDet3 = sampler_state
{
    Texture = texDet3;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};
sampler2D samTexDet4 = sampler_state
{
    Texture = texDet4;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};

sampler2D samTexColor  = sampler_state
{
    Texture = texColor;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};
sampler2D samTexNorm  = sampler_state
{
    Texture = texNorm;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};
sampler2D samTexShd  = sampler_state
{
    Texture = texShd;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};

sampler2D samTexCliff  = sampler_state
{
    Texture = texCliff;
    MinFilter = LINEAR;
    MagFilter = LINEAR;    
    MipFilter = NONE;
	
	AddressU = WRAP;
	AddressV = WRAP;
};

float4 k_d;
float4 k_a;

float4 i_a;
float4 i_d;

struct PSInput
{
    float2 GlobeCoord : TEXCOORD0; 
    float2 DetailCoord : TEXCOORD1; 

    float4 smLgtPos : TEXCOORD2;
    float Depth : TEXCOORD3;
	float3 TangentSpaceLDir : TEXCOORD4;
    float2 Height_Blend : TEXCOORD5;
};

struct PSOutput
{
    float4 Color : COLOR;
};



PSOutput PSMain(PSInput ip)
{
    PSOutput o;
    
    float3 N = 2 * (float3)tex2D(samTexNorm, ip.GlobeCoord) - 1;
    
    float ndl = dot(N, ip.TangentSpaceLDir);
	
	float4 index = tex2D(samTexIndex, ip.GlobeCoord);
	
	float4 color = 0;
	
	if (index[0]>0.01)
	{
		color += (index[0]) * tex2D(samTexDet1, ip.DetailCoord);
		//color += (index[0]) * tex2D(samTexDet1, ip.DetailCoord*7);
	}
	if (index[1]>0.01)
	{
		color += (index[1]) * tex2D(samTexDet2, ip.DetailCoord);
		//color += (index[1]) * tex2D(samTexDet2, ip.DetailCoord*7);
	}
	if (index[2]>0.01)
	{
		color += (index[2]) * tex2D(samTexDet3, ip.DetailCoord);
		//color += (index[2]) * tex2D(samTexDet3, ip.DetailCoord*7);
	}
	if (index[3]>0.01)
	{
		color += (index[3]) * tex2D(samTexDet4, ip.DetailCoord);
		//color += (index[3]) * tex2D(samTexDet4, ip.DetailCoord*7);
	}
	
    o.Color = lerp(color , tex2D(samTexColor, ip.GlobeCoord), ip.Height_Blend.y);

    float wgt = saturate(N.z);
    
    if (wgt > 0.2f)
    {
		o.Color.rgb = lerp(tex2D(samTexCliff, ip.DetailCoord*1).rgb, o.Color.rgb, (wgt - 0.2)/0.8);
    }
    
    
	float4 amb = i_a * k_a;
	float4 dif = i_d * k_d;
	dif.xyz *= max(0, ndl);
	
    float2 ShadowTexC = (ip.smLgtPos.xy / ip.smLgtPos.w) * 0.5 + float2( 0.5, 0.5 );
    ShadowTexC.y = 1.0f - ShadowTexC.y;

	float shd = 1;//VSM_FILTER(samTexShd, ShadowTexC, ip.smLgtPos.z);
	
    o.Color = o.Color * (amb + dif );
   
   
    o.Color.rgb *= shd;
    
    //o.Color = GetColor(o.Color, ip.Height_Blend.x);
    o.Color = float4(0.5*(N+1),1);
    return o;
}


technique TerrainRendering
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VSMain();
        PixelShader = compile ps_3_0 PSMain();
    }
}
