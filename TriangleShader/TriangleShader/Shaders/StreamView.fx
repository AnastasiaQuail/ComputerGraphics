struct ConstData
{
	float4x4 WorldViewProj;
	float4x4 World;
	float4x4 InvertWorld;
	float4 ViewPos;
};
cbuffer VS_CONSTANT_BUFFER : register(b0)
{
	ConstData data;
};
struct VS_IN
{
	float4 pos:POSITION;
	float4 col:COLOR;
};
struct PS_IN
{
	float4 pos:SV_POSITION;
	float4 col:COLOR;
};
PS_IN VSMain(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.pos = input.pos;
	output.col = input.col;

	return output;
}
float4 PSMain(PS_IN input) : SV_Target
{
	return input.col;
}