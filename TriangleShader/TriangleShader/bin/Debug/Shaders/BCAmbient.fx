Texture2D Picture : register(t0);
SamplerState Sampler : register(s0);

Texture2D ShadowMap : register(t1);
SamplerState ShadowSampler : register(s1);

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
	float4 norm : NORMAL;
	float4 tex : TEXCOORD;
};
struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 norm : NORMAL;
	float4 tex : TEXCOORD0;
	float4 posWorld : TEXCOORD1;

};
PS_IN VSMain(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(float4(input.pos.xyz, 1.0f), data.WorldViewProj);
	output.norm = normalize(mul(input.norm, data.InvertWorld));
	output.tex = input.tex;
	output.posWorld = mul(float4(input.pos.xyz, 1.0f), data.World);

	return output;
}
float4 PSMain(PS_IN input) : SV_Target
{
float4 result;
float ambient = 0.1f;

float4 kd = Picture.Sample(Sampler, input.tex.xy);
result = kd*ambient;
return float4(result.rgb,1.0f);
}
