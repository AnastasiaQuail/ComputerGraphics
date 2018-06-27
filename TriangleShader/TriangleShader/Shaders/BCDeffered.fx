SamplerState Sampler : register(s0);

Texture2D Picture : register(t0);
Texture2D TexDiffuse: register(t1);
Texture2D TexNormal :register(t2);
Texture2D TexPosition: register(t3);
struct ConstData
{
	float4x4 WorldViewProj;
	float4x4 World;
	float4x4 InvertWorld;
	float4 ViewPos;
	float4x4 InverseProjectionView; 
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
    float4 norm : NORMAL;
    float4 tex : TEXCOORD0;
};
struct VS_OUT
{
    float4 pos : SV_POSITION; ////
    float4 norm : NORMAL;
    float4 tex : TEXCOORD0;
};


struct PS_DEF_OUT
{
    float4 diffuse : SV_Target0;
    float4 normal : SV_Target1;
    float4 position : SV_Target2;
	//float4 debug: SV_Target3;
};



///////////////////////////////////////////////// G pass /////////////////////////////////////////
VS_OUT VSMain(VS_IN vIn)
{
	VS_OUT vOut = (VS_OUT) 0;

	vOut.pos = mul(float4(vIn.pos.xyz, 1.0f), data.WorldViewProj);
	vOut.norm = normalize(mul(vIn.norm, data.InvertWorld));
	vOut.tex = vIn.tex;/*
	float4x4 lightWorldViewProj = mul(data.World, light.ViewProj);
	float4 posLight = mul(float4(vIn.pos.xyz, 1.0f), lightWorldViewProj);
	posLight = posLight / posLight.w;
	vOut.posLight = posLight;*/

	return vOut;
}

PS_DEF_OUT PSMain(VS_OUT pIn)
{
	PS_DEF_OUT pOut = (PS_DEF_OUT) 0;
	pOut.diffuse = Picture.Sample(Sampler, pIn.tex.xy);
    pOut.normal = pIn.norm;
	pOut.position = pIn.pos;
	//pOut.debug = pIn.pos;
	return pOut;
}
///////////////////////////////////////////////////////////////////////////////////////////////////////////
