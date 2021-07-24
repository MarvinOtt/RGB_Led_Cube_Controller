#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

Texture2D layertex;
sampler2D LayerSampler = sampler_state
{
	Texture = <layertex>;
	addressU = Clamp;
	addressV = Clamp;
	mipfilter = NONE;
	minfilter = POINT;
	magfilter = POINT;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 OUT;
	if ((input.TextureCoordinates.x % 0.125f > 0.0f && input.TextureCoordinates.x % 0.125f < 0.002f) || (input.TextureCoordinates.y % 0.125f > 0.0f && input.TextureCoordinates.y % 0.125f < 0.002f) || input.TextureCoordinates.x > 0.998f || input.TextureCoordinates.y > 0.998f)
	{
		return float4(0.2f, 0.2f, 0.2f, 1.0f);
	}
	OUT = tex2D(SpriteTextureSampler, input.TextureCoordinates) * 0.0001f + tex2D(LayerSampler, input.TextureCoordinates);
	OUT.a = 1.0f;
	return OUT;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};