#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

matrix World;
matrix View;
matrix Projection;
float3 lightdir;
float3 ledcol;

struct VertexShaderInput
{
	float3 Position : SV_POSITION;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 Normal : NORMAL0;
	float4 worldpos : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 worldPosition = mul(float4(input.Position, 1.0f), World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.Normal = input.Normal;
	output.worldpos = worldPosition;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float refstrength_diffuse = clamp(dot(lightdir, input.Normal), 0.0f, 1.0f);

return float4(ledcol, 1.0f) *(0.8f + refstrength_diffuse * 0.2f);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};