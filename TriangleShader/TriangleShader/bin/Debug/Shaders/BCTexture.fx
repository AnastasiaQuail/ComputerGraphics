Texture2D Picture : register(t0);
SamplerState Sampler : register(s0);

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
	float4 pos : POSITION;
	float4 tex : TEXCOORD;
};
struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 tex : TEXCOORD;
};
PS_IN VSMain (VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(input.pos, data.WorldViewProj);
	output.tex = input.tex;

	return output;
}
float4 PSMain (PS_IN input): SV_Target
{
return Picture.Sample(Sampler,input.tex.xy);
}