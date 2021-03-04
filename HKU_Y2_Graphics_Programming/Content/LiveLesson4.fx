#if OPENGL
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// External Properties
float4x4 World;
float4x4 View;
float4x4 Projection;

float3 LightDirection;
float3 ViewDirection;
float3 Ambient;

float3 CameraPosition;
float Time;

Texture2D DirtTex;
sampler2D DirtTextureSampler = sampler_state
{
    Texture = <DirtTex>;
    MipFilter = LINEAR;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

Texture2D WaterTex;
sampler2D WaterTextureSampler = sampler_state
{
    Texture = <WaterTex>;
    MipFilter = LINEAR;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

Texture2D GrassTex;
sampler2D GrassTextureSampler = sampler_state
{
    Texture = <GrassTex>;
    MipFilter = LINEAR;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

Texture2D RockTex;
sampler2D RockTextureSampler = sampler_state
{
    Texture = <RockTex>;
    MipFilter = LINEAR;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

Texture2D SnowTex;
sampler2D SnowTextureSampler = sampler_state
{
    Texture = <SnowTex>;
    MipFilter = LINEAR;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

TextureCube SkyTex;
samplerCUBE SkyTextureSampler = sampler_state
{
    Texture = <SkyTex>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = Mirror;
    AddressV = Mirror;
};


// Getting out vertex data from vertex shader to pixel shader
struct VertexShaderOutput {
    float4 position : SV_POSITION;
    float4 color : COLOR0;
    float2 tex : TEXCOORD0;
    float3 normal : TEXCOORD1;
    float3 worldPos : TEXCOORD2;
    float3 texCube : TEXCOORD3;
    float3 binormal : TEXCOORD4;
    float3 tangent : TEXCOORD5;
};

// Vertex shader, receives values directly from semantic channels
VertexShaderOutput MainVS( float4 position : POSITION, float4 color : COLOR0, float3 normal : NORMAL, float3 binormal : BINORMAL, float3 tangent : TANGENT, float2 tex : TEXCOORD0 )
{
    VertexShaderOutput o = (VertexShaderOutput)0;

    o.position = mul( mul( mul(position, World), View ), Projection );
    o.color = color;
    o.normal = mul(normal, (float3x3)World);
    o.tex = tex;
    o.worldPos = mul(position, World);

    return o;
}

// Pixel Shader, receives input from vertex shader, and outputs to COLOR semantic
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float d = distance(input.worldPos, CameraPosition);

    // Sample texure Dirt
    float3 dirtFar = tex2D(DirtTextureSampler, input.tex / 10).rgb;
    float dirtClose = tex2D(DirtTextureSampler, input.tex * 50).rgb;
    float3 dirt = lerp(dirtClose, dirtFar, clamp(d / 250, 0, 1));

    // water
    float3 waterFar = tex2D(WaterTextureSampler, input.tex / 10).rgb;
    float waterClose = tex2D(WaterTextureSampler, input.tex * 50).rgb;
    float3 water = lerp(waterClose, waterFar, clamp(d / 250, 0, 1));//float3(0, 0, 1);

    // grass
    float3 grassFar = tex2D(GrassTextureSampler, input.tex / 10).rgb;
    float grassClose = tex2D(GrassTextureSampler, input.tex * 50).rgb;
    float3 grass = lerp(grassClose, grassFar, clamp(d / 250, 0, 1));// float3(0, 1, 0);

    // rock
    float3 rockFar = tex2D(RockTextureSampler, input.tex / 10).rgb;
    float rockClose = tex2D(RockTextureSampler, input.tex * 50).rgb;
    float3 rock = lerp(rockClose, rockFar, clamp(d / 250, 0, 1)); //float3(0.5, 0.5, 0.5);

    // snow
    float3 snowFar = tex2D(SnowTextureSampler, input.tex / 10).rgb;
    float snowClose = tex2D(SnowTextureSampler, input.tex * 50).rgb;
    float3 snow = lerp(snowClose, snowFar, clamp(d / 250, 0, 1));// float3(1, 1, 1);

    // Weights
    float wd = clamp((input.worldPos.y - 50) / 10, -1, 1) * 0.5 + 0.5;
    float dg = clamp((input.worldPos.y - 100) / 10, -1, 1) * 0.5 + 0.5;
    float gr = clamp((input.worldPos.y - 150) / 10, -1, 1) * 0.5 + 0.5;
    float rs = clamp((input.worldPos.y - 200) / 10, -1, 1) * 0.5 + 0.5;

    float3 texColor = lerp(lerp(lerp(lerp(water, dirt, wd), grass, dg), rock, gr), snow, rs);

    // Lighting calculation
    float3 lighting = max( dot(input.normal, LightDirection), 0.0) + Ambient;

    float fogAmount = clamp((d - 250) / 1500, 0, 1);
    float3 fogColor = float3(188, 214, 231) / 255.0;

    // Output
    return float4(lerp(texColor * lighting, fogColor, fogAmount), 1);
}

VertexShaderOutput SkyVS(float4 position : POSITION, float3 normal : NORMAL, float2 tex : TEXCOORD0)
{
    VertexShaderOutput o = (VertexShaderOutput)0;

    o.position = mul(mul(mul(position, World), View), Projection);
    o.normal = mul(normal, (float3x3)World);
    o.worldPos = mul(position, World);

    float4 VertexPosition = mul(position, World);
    o.texCube = normalize(VertexPosition - CameraPosition);

    return o;
}

float4 SkyPS(VertexShaderOutput input) : COLOR
{
    float3 topColor = float3(68, 118, 189) / 255.0;
    float3 botColor = float3(188, 214, 231) / 255.0;

    float3 viewDirection = normalize(input.worldPos - CameraPosition);

    float sun = pow(max(dot(-viewDirection, LightDirection), 0.0), 128);
    float3 sunColor = float3(255, 200, 50) / 225.0;

    return float4(lerp(botColor, topColor, viewDirection.y) + sun * sunColor, 1);
}

float4 UnlitTransparentPS(VertexShaderOutput input) : COLOR
{
    float4 texColor = tex2D(GrassTextureSampler, input.tex);

    // if alpha < 0.5, discard
    //if (texColor.a - 0.5 < 0) discard;
    clip(texColor.a - 0.5);

    return texColor;
}

technique UnlitTransparent
{
    pass
    {
        /*AlphaBlendEnable = true;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;*/

        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL UnlitTransparentPS();
    }
};

technique Terrain
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

technique SkyBox
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL SkyVS();
        PixelShader = compile PS_SHADERMODEL SkyPS();
    }
};