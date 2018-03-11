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
	float4 tex : TEXCOORD;
};

struct VS_OUT
{
	float4 pos : SV_POSITION;
	float4 norm : NORMAL;
	float4 tex : TEXCOORD0;
	float4 posWorld : TEXCOORD1;
	float4 posLight : TEXCOORD2;
};

struct PS_DEF_OUT
{
	float4 diffuse  : SV_Target0;
	float4 normal   : SV_Target1;
	float4 position : SV_Target2;
};

struct PS_OUT
{
	float4 color : SV_Target;
};


VS_OUT VSMain(VS_IN vIn)
{
	VS_OUT vOut;
	
	vOut.pos = mul(float4(vIn.pos.xyz, 1.0f), data.WorldViewProj);
	vOut.norm = normalize(mul(vIn.norm, data.InvertWorld));
	vOut.tex = vIn.tex;
	vOut.posWorld = mul(float4(vIn.pos.xyz, 1.0f), data.World);
	float4x4 lightWorldViewProj = mul(data.World, light.ViewProj);
	float4 posLight = mul(float4(vIn.pos.xyz, 1.0f), lightWorldViewProj);
	posLight = posLight / posLight.w;
	vOut.posLight = posLight;

	return vOut;
}

PS_DEF_OUT PSMain(VS_OUT pIn)
{
	PS_DEF_OUT pOut;
	pOut.diffuse = Picture.Sample(Sampler, pIn.tex.xy);
	pOut.normal = pIn.norm;
	pOut.position = pIn.pos;
	return pOut;
}

VS_OUT LightingVS(VS_IN vIn)
{
	float4 ScreenToView(float4 screen) {
		float2 texCoord = screen.xy / 800f;
		float4 clip = float4(float2(vIn.tex.x, 1.0f - vIn.tex.y) * 2.0f - 1.0f, screen.z, screen.w);
		// View space position.
		float4 view = mul(data.InverseProjectionView, clip);
		// Perspective projection.
		view = view / view.w;
		return view;
	}
	VS_OUT vOut;
	vOut.pos = vIn.pos;
	vOut.norm =ScreenToView(TexNormal.Sample(Sampler, vIn.tex.xy));
	vOut.tex = vIn.tex;
	vOut.posWorld = ScreenToView(TexPosition.Sample(Sampler, vIn.tex.xy));
	float4x4 lightWorldViewProj = mul(data.World, light.ViewProj);
	float4 posLight = mul(float4(vIn.pos.xyz, 1.0f), lightWorldViewProj);
	posLight = posLight / posLight.w;
	vOut.posLight = posLight;

	return vOut;
}

PS_OUT LightingPS(VS_OUT pIn)
{
	PS_OUT pOut;
	float4 light_pos = pIn.posLight;
	float4 normal = pIn.norm;
	float4 ambient = TexDiffuse.Sample(Sampler, pIn.tex.xy)*0.1f;

	if (length(normal) > 0.0f)
	{
		float4 V = data.ViewPos - VS_OUT.pos;
		float4 L = light_pos + float4(0f, 0f, -1f, 0f);
		float4 R = normalize(reflect(-L, normal));
		float RdotV = max(dot(R, V), 0);
		specular = light.col * pow(RdotV, 200f);

		l = normalize(light.pos - pIn.posWorld);
		diffuse = max(0, dot(l, normal));

		float3 colorLinear = diffuse*ambient + specular;
		pOut.color = float4(pow(colorLinear, float3(1.0f / 2.2f, 1.0f / 2.2f, 1.0f / 2.2f)), 1.0f);
		return pOut;
	}

	pOut.color = float4(ambient, 1.0f);
	return pOut;
}
