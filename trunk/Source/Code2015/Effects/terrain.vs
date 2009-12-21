//Y = y + r - sqrt(r*r-rh*rh);
//rh*rh = x*x + y*y
#define radius 6731
#define halfTile 512.5



float4x4 mvp : register(c0);

struct VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord1 : TEXCOORD0; 
};
struct VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord1 : TEXCOORD0; 
};

VSOutput main(VSInput ip)
{
    VSOutput o;

    float rx = ip.Position.x-halfTile;
    float ry = ip.Position.z-halfTile;

    float rh = rx*rx + ry*ry;

    ip.Position.y += sqrt(radius*radius - rh)-radius;
    o.Position = mul(ip.Position, mvp);
    o.TexCoord1 = ip.TexCoord1;
    return o;
}
