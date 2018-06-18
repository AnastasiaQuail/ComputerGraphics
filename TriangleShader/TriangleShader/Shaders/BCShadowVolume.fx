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
struct GS_IN
{
	float4 pos:POSITION;
	float4 col:COLOR;
};
struct VS_IN
{
	float4 pos:POSITION;
	float4 col: COLOR;
};
float3 CrossProduct(float4 left, float4 right)
{
    float3 returnValue;
    returnValue.x = left.y * right.z - left.z * right.y;
    returnValue.y = left.z * right.x - left.x * right.z;
    returnValue.z = left.x * right.y - left.y * right.x;
    return returnValue;
}
GS_IN ToInf(GS_IN p) {
	float4 t = (-light.pos + p.pos) + p.pos;
    GS_IN inf1;
	inf1.pos = float4(t.x, t.y, t.z, 0.f);
	inf1.col = p.col;
	return inf1;
}
GS_IN VSMain(VS_IN input)
{
	GS_IN output = (GS_IN)0;

	output.pos = input.pos;
	output.col = input.col;
}
[maxvertexcount(12)]
void GSStream(point GS_IN input[6], inout TriangleStream<GS_IN> stream) {

	GS_IN p1 = input[0];
	GS_IN p2 = input[1];
	GS_IN p3 = input[2];
	GS_IN p4 = input[3];
	GS_IN p5 = input[4];
	GS_IN p6 = input[5];

	GS_IN inf1, inf2;
    float3 norm1 = 0;
    float3 norm2 = 0;
    float3 norm3 = 0;
    float3 norm = 0;
    float4 p, q;

	if ((p2.pos != p4.pos)&(p1.pos!= p4.pos)) {
		p = p2.pos - p4.pos;
		q = p1.pos - p4.pos;
		norm1 = CrossProduct(p, q);
	}

	if ((p2.pos != p6.pos)&(p2.pos!= p3.pos)) {
		p = p6.pos - p2.pos;
		q = p3.pos - p2.pos;
        norm2 = CrossProduct(p, q);
    }

	if ((p6.pos != p4.pos)&(p5.pos!= p4.pos)) {
		p = p6.pos - p4.pos;
		q = p5.pos - p4.pos;
		norm3 = CrossProduct(p, q);
	}

	if ((p2.pos != p4.pos)&(p6.pos!= p4.pos)) {
		p = p2.pos - p4.pos;
		q = p6.pos - p4.pos;
		norm = CrossProduct(p, q);
	}

	if (dot(norm2, norm) < 0)
	{
		stream.Append(p2);
		stream.Append(p6);
		stream.Append(ToInf(p2));
		stream.Append(ToInf(p6));
	}
	if (dot(norm3, norm) < 0)
	{
		stream.Append(p4);
		stream.Append(p6);
		stream.Append(ToInf(p4));
		stream.Append(ToInf(p6));
	}
	if (dot(norm1, norm) < 0)
	{
		stream.Append(p2);
		stream.Append(p4);
		stream.Append(ToInf(p2));
		stream.Append(ToInf(p4));
	}
}