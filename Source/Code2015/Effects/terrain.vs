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

    float rx = ip.Position.X - halfTile;
    float ry = ip.Position.Z - halfTile;

    float rh = halfTile - sqrt(rx*rx + ry*ry);

    ip.Position.Y += r-sqrt(radius*radius - rh*rh);
    o.Position = mul(ip.Position, mvp);
    o.TexCoord1 = ip.TexCoord1;
    return o;
}
