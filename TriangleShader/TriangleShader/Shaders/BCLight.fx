Texture2D Picture : register(t0);
SamplerState Sampler : register(s0);

struct ConstData
{
	float4x4 WorldViewProj;
	float4x4 World;
	float4x4 InvertWorld;
	float4 ViewPos;
}; 
struct LightData
{
	float4 pos : POSITION;
	float4 col : COLOR;
	float4x4 ViewProj;
};
cbuffer VS_CONSTANT_BUFFER : register(b0)
{
	ConstData data;
};
cbuffer Light_CONSTANT_BUFFER : register(b1)
{
	LightData light;
};
struct VS_IN
{
	float4 pos : POSITION;
};
struct VS_OUT
{
	float4 pos : SV_POSITION;
};
VS_OUT VSMain(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;

	float4x4 lightWorldViewProj = mul(data.World, light.ViewProj);
	float4 worldPosition = mul(float4(input.pos.xyz,1.0f), lightWorldViewProj);
	output.pos = worldPosition;

	return output;
}

