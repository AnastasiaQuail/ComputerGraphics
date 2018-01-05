cbuffer VS_CONSTANT_BUFFER : register(b0)
{
float4x4 WorldViewProj;
};
struct VS_IN
{
float4 pos:POSITION;
float4 col: COLOR;
};
struct PS_IN
{
float4 pos:SV_POSITION;
float4 col: COLOR;
};
PS_IN VSMain (VS_IN input)
{
PS_IN output = (PS_IN)0;

output.pos =mul(input.pos,WorldViewProj);
output.col = input.col;

return output;
}
float4 PSMain (PS_IN input): SV_Target
{
return input.col;
}