SamplerState Sampler : register(s0);

Texture2D Picture : register(t0);
Texture2D TexDiffuse : register(t1);
Texture2D TexNormal : register(t2);
Texture2D TexPosition : register(t3);

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

struct PS_IN
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
};
struct PS_IN_Light
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD0;
    float4 posLight : TEXCOORD1;
};
float4 ScreenToView(float4 screen, float2 tex)
{
    float2 texCoord = screen.xy / 800.f;
    float4 clip = float4(float2(tex.x, 1.0f - tex.y) * 2.0f - 1.0f, screen.z, screen.w);
	// View space position.
    float4 view = mul(data.InverseProjectionView, clip);
	// Perspective projection.
    view = view / view.w;
    return view;
}
PS_IN VSMain(uint id: SV_VertexID)
{
    PS_IN output = (PS_IN) 0;

	output.tex = float2((id << 1) & 2, id & 2);
    output.pos = float4(output.tex * float2(2, -2) + float2(-1, 1), 0, 1);
    
    return output;
}
float4 PSMain(PS_IN input) : SV_Target0
{
    float4 ambient = TexDiffuse.Sample(Sampler, input.tex.xy);
	ambient = ambient * 0.5f;
    float4 result  = float4(ambient.rgb, 1.0f);
    return result;
}
PS_IN_Light LightingVS(VS_IN input)
{
    PS_IN_Light output = (PS_IN_Light) 0;

    output.pos = mul(float4(input.pos.xyz, 1.0f), data.WorldViewProj);
    output.tex = input.tex;
    float4x4 lightWorldViewProj = mul(data.World, light.ViewProj);
    float4 posLight = mul(float4(output.pos.xyz, 1.0f), lightWorldViewProj);
    posLight = posLight / posLight.w;
    output.posLight = posLight;
    return output;
}

float4 LightingPS(PS_IN_Light pIn) : SV_Target0
{
    float4 light_pos = pIn.posLight;
    float4 eyePos = { 0, 0, 0, 1 };
    
    float4 ambient = TexDiffuse.Load(int3(pIn.pos.xy,0));
    float4 specular;
    float4 diffuse;
    float4 l;
	float4 tex = TexPosition.Load(int3(pIn.pos.xy,0));
	float depth = tex.z;
    float4 P = ScreenToView(float4(tex.xy,depth,1.0f), pIn.pos);
	float4 normal = TexNormal.Load(int3(tex.xy,0));
    float4 color;

	if (length(normal) > 0.0f)
	{
		float4 V = normalize(eyePos - P);
		float4 L = light_pos + float4(0.f, 0.f, -1.f, 0.f);
		float4 R = normalize(reflect(-L, normal));
		float RdotV = max(dot(R, V), 0);
		specular = light.col * RdotV;

		l = normalize(light.pos - P);
		diffuse = max(0, dot(l, normal));

		float4 colorLinear = diffuse * ambient + specular;
		color = float4(pow(colorLinear, float4(1.0f / 2.2f, 1.0f / 2.2f, 1.0f / 2.2f, 1.f)));
		return color;
	}

    color = ambient;
    return color;
}
float4 PSDirect(PS_IN pIn) : SV_Target0
{
	float4 ambient = TexDiffuse.Sample(Sampler, pIn.tex.xy);
	float4 diffuse;
	float4 l;
	float4 pos = TexPosition.Sample(Sampler, pIn.tex.xy);
	float4 normal = TexNormal.Sample(Sampler, pIn.tex.xy);
	float4 color;
	/*if (length(normal) > 0.0f)
	{*/
		l = normalize(light.pos - pos);
		diffuse = max(0, dot(l, normal));

		float4 result = ambient * (diffuse)*light.col;
		return float4(result.rgb, 1.0f);
	/*}
	else {
		return float4(0.0f, 0.0f, 0.0f, 1.0f);
	}*/
	
}



