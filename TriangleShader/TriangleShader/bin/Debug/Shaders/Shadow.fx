cbuffer VS_CONSTANT_BUFFER : register(b0)
{
float4x4 WorldViewProj;
};
struct VS_IN
{
float4 pos:POSITION;
};
struct PS_IN
{
float4 pos:SV_POSITION;
};
PS_IN VSMain (VS_IN input)
{
PS_IN output = (PS_IN)0;

output.pos =mul(input.pos,WorldViewProj);

return output;
}
float4 PSMain (PS_IN input): SV_Target
{
return input.col;
}