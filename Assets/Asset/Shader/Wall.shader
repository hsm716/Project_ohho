Shader "Wall"
{
	Properties
	{
		Color_afbdcc2f0d024fe08de994e91925be3b("Tint", Color) = (0, 0, 0, 0)
		Vector1_1("Smoothness", Range(0, 1)) = 0.5
		_PlayerPosition("Player Position", Vector) = (0.5, 0.5, 0, 0)
		_Size("Size", Float) = 1
		Vector1_ae10dd6a890d4bb6bcc68363347ade9a("Opacity", Range(0, 1)) = 1
		[NoScaleOffset]Texture2D_3aaa6060906e457383ff3ad5809e2d06("Base Map", 2D) = "white" {}
		[NoScaleOffset]Texture2D_b604a5e590b648cb846f3bfebb35331c("Metallic Map", 2D) = "white" {}
		[NoScaleOffset]Texture2D_b0650479ef3a4d8e95897f48d47256e8("Normal Map", 2D) = "bump" {}
		[NoScaleOffset]Texture2D_bbb0f0de226b483e97dfd4fac79cff11("Occulusion Map", 2D) = "white" {}
		[NoScaleOffset]Texture2D_933df24ac27a484f9b6e59168017b62d("Emission Map", 2D) = "white" {}
		[HDR]Color_a5bb6ace4e044cee85204dac7bccd927("Emission Color", Color) = (0, 0, 0, 0)
		Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d("Metallic", Range(0, 1)) = 0.5
		Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42("CircleSmoothness", Range(0, 1)) = 0.5
		[HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}
		SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType" = "Transparent"
			"UniversalMaterialType" = "Lit"
			"Queue" = "Transparent"
		}
		Pass
		{
			Name "Universal Forward"
			Tags
			{
				"LightMode" = "UniversalForward"
			}

		// Render State
		Cull Back
	Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
	ZTest LEqual
	ZWrite On

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 4.5
	#pragma exclude_renderers gles gles3 glcore
	#pragma multi_compile_instancing
	#pragma multi_compile_fog
	#pragma multi_compile _ DOTS_INSTANCING_ON
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
	#pragma multi_compile _ LIGHTMAP_ON
	#pragma multi_compile _ DIRLIGHTMAP_COMBINED
	#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
	#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
	#pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
	#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
	#pragma multi_compile _ _SHADOWS_SOFT
	#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
	#pragma multi_compile _ SHADOWS_SHADOWMASK
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define ATTRIBUTES_NEED_TEXCOORD1
		#define VARYINGS_NEED_POSITION_WS
		#define VARYINGS_NEED_NORMAL_WS
		#define VARYINGS_NEED_TANGENT_WS
		#define VARYINGS_NEED_TEXCOORD0
		#define VARYINGS_NEED_VIEWDIRECTION_WS
		#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_FORWARD
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		float4 uv0 : TEXCOORD0;
		float4 uv1 : TEXCOORD1;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		float3 normalWS;
		float4 tangentWS;
		float4 texCoord0;
		float3 viewDirectionWS;
		#if defined(LIGHTMAP_ON)
		float2 lightmapUV;
		#endif
		#if !defined(LIGHTMAP_ON)
		float3 sh;
		#endif
		float4 fogFactorAndVertexLight;
		float4 shadowCoord;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 TangentSpaceNormal;
		float3 WorldSpacePosition;
		float4 ScreenPosition;
		float4 uv0;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		float3 interp1 : TEXCOORD1;
		float4 interp2 : TEXCOORD2;
		float4 interp3 : TEXCOORD3;
		float3 interp4 : TEXCOORD4;
		#if defined(LIGHTMAP_ON)
		float2 interp5 : TEXCOORD5;
		#endif
		#if !defined(LIGHTMAP_ON)
		float3 interp6 : TEXCOORD6;
		#endif
		float4 interp7 : TEXCOORD7;
		float4 interp8 : TEXCOORD8;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		output.interp1.xyz = input.normalWS;
		output.interp2.xyzw = input.tangentWS;
		output.interp3.xyzw = input.texCoord0;
		output.interp4.xyz = input.viewDirectionWS;
		#if defined(LIGHTMAP_ON)
		output.interp5.xy = input.lightmapUV;
		#endif
		#if !defined(LIGHTMAP_ON)
		output.interp6.xyz = input.sh;
		#endif
		output.interp7.xyzw = input.fogFactorAndVertexLight;
		output.interp8.xyzw = input.shadowCoord;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		output.normalWS = input.interp1.xyz;
		output.tangentWS = input.interp2.xyzw;
		output.texCoord0 = input.interp3.xyzw;
		output.viewDirectionWS = input.interp4.xyz;
		#if defined(LIGHTMAP_ON)
		output.lightmapUV = input.interp5.xy;
		#endif
		#if !defined(LIGHTMAP_ON)
		output.sh = input.interp6.xyz;
		#endif
		output.fogFactorAndVertexLight = input.interp7.xyzw;
		output.shadowCoord = input.interp8.xyzw;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
	Out = A + B;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
	Out = A * B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float3 BaseColor;
	float3 NormalTS;
	float3 Emission;
	float Metallic;
	float Smoothness;
	float Occlusion;
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	float4 _Property_2165fc3283a241e482d281330b4086e1_Out_0 = Color_afbdcc2f0d024fe08de994e91925be3b;
	UnityTexture2D _Property_2ef9f30408844319bfcc4bea45316d28_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
	float4 _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2ef9f30408844319bfcc4bea45316d28_Out_0.tex, _Property_2ef9f30408844319bfcc4bea45316d28_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_R_4 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.r;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_G_5 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.g;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_B_6 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.b;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_A_7 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.a;
	float4 _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2;
	Unity_Add_float4(_Property_2165fc3283a241e482d281330b4086e1_Out_0, _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0, _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2);
	UnityTexture2D _Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
	float4 _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0 = SAMPLE_TEXTURE2D(_Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0.tex, _Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0.samplerstate, IN.uv0.xy);
	_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0);
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_R_4 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.r;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_G_5 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.g;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_B_6 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.b;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_A_7 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.a;
	UnityTexture2D _Property_4582d80c094d4b188e907208ff0a494a_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_933df24ac27a484f9b6e59168017b62d);
	float4 _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4582d80c094d4b188e907208ff0a494a_Out_0.tex, _Property_4582d80c094d4b188e907208ff0a494a_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_R_4 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.r;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_G_5 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.g;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_B_6 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.b;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_A_7 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.a;
	float4 _Property_1a42c57c811b4b5bb19f39dd500bdbc9_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_a5bb6ace4e044cee85204dac7bccd927) : Color_a5bb6ace4e044cee85204dac7bccd927;
	float4 _Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2;
	Unity_Multiply_float(_SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0, _Property_1a42c57c811b4b5bb19f39dd500bdbc9_Out_0, _Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2);
	float _Property_81611d5549fd4509b86de5c2aa9f6f4d_Out_0 = Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
	UnityTexture2D _Property_4af1499beb0a4526bb400fbc58d1b006_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_b604a5e590b648cb846f3bfebb35331c);
	float4 _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4af1499beb0a4526bb400fbc58d1b006_Out_0.tex, _Property_4af1499beb0a4526bb400fbc58d1b006_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_f453c42eef784618af6744ea9794da15_R_4 = _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0.r;
	float _SampleTexture2D_f453c42eef784618af6744ea9794da15_G_5 = _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0.g;
	float _SampleTexture2D_f453c42eef784618af6744ea9794da15_B_6 = _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0.b;
	float _SampleTexture2D_f453c42eef784618af6744ea9794da15_A_7 = _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0.a;
	float4 _Multiply_16bc27119a6845e99c0147374e4c55c8_Out_2;
	Unity_Multiply_float((_Property_81611d5549fd4509b86de5c2aa9f6f4d_Out_0.xxxx), _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0, _Multiply_16bc27119a6845e99c0147374e4c55c8_Out_2);
	float _Property_921137aef1624dea8e57841a867425e2_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float _Multiply_e9d1907defde412bb1429f82e8510226_Out_2;
	Unity_Multiply_float(_Property_921137aef1624dea8e57841a867425e2_Out_0, _SampleTexture2D_f453c42eef784618af6744ea9794da15_A_7, _Multiply_e9d1907defde412bb1429f82e8510226_Out_2);
	UnityTexture2D _Property_d2249d799da3488d874fcdb0e76ba2f5_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
	float4 _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d2249d799da3488d874fcdb0e76ba2f5_Out_0.tex, _Property_d2249d799da3488d874fcdb0e76ba2f5_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_R_4 = _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0.r;
	float _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_G_5 = _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0.g;
	float _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_B_6 = _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0.b;
	float _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_A_7 = _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0.a;
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.BaseColor = (_Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2.xyz);
	surface.NormalTS = (_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.xyz);
	surface.Emission = (_Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2.xyz);
	surface.Metallic = (_Multiply_16bc27119a6845e99c0147374e4c55c8_Out_2).x;
	surface.Smoothness = _Multiply_e9d1907defde412bb1429f82e8510226_Out_2;
	surface.Occlusion = (_SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0).x;
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



	output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
	output.uv0 = input.texCoord0;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

	ENDHLSL
}
Pass
{
	Name "GBuffer"
	Tags
	{
		"LightMode" = "UniversalGBuffer"
	}

		// Render State
		Cull Back
	Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
	ZTest LEqual
	ZWrite Off

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 4.5
	#pragma exclude_renderers gles gles3 glcore
	#pragma multi_compile_instancing
	#pragma multi_compile_fog
	#pragma multi_compile _ DOTS_INSTANCING_ON
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		#pragma multi_compile _ LIGHTMAP_ON
	#pragma multi_compile _ DIRLIGHTMAP_COMBINED
	#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
	#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
	#pragma multi_compile _ _SHADOWS_SOFT
	#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
	#pragma multi_compile _ _GBUFFER_NORMALS_OCT
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define ATTRIBUTES_NEED_TEXCOORD1
		#define VARYINGS_NEED_POSITION_WS
		#define VARYINGS_NEED_NORMAL_WS
		#define VARYINGS_NEED_TANGENT_WS
		#define VARYINGS_NEED_TEXCOORD0
		#define VARYINGS_NEED_VIEWDIRECTION_WS
		#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_GBUFFER
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		float4 uv0 : TEXCOORD0;
		float4 uv1 : TEXCOORD1;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		float3 normalWS;
		float4 tangentWS;
		float4 texCoord0;
		float3 viewDirectionWS;
		#if defined(LIGHTMAP_ON)
		float2 lightmapUV;
		#endif
		#if !defined(LIGHTMAP_ON)
		float3 sh;
		#endif
		float4 fogFactorAndVertexLight;
		float4 shadowCoord;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 TangentSpaceNormal;
		float3 WorldSpacePosition;
		float4 ScreenPosition;
		float4 uv0;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		float3 interp1 : TEXCOORD1;
		float4 interp2 : TEXCOORD2;
		float4 interp3 : TEXCOORD3;
		float3 interp4 : TEXCOORD4;
		#if defined(LIGHTMAP_ON)
		float2 interp5 : TEXCOORD5;
		#endif
		#if !defined(LIGHTMAP_ON)
		float3 interp6 : TEXCOORD6;
		#endif
		float4 interp7 : TEXCOORD7;
		float4 interp8 : TEXCOORD8;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		output.interp1.xyz = input.normalWS;
		output.interp2.xyzw = input.tangentWS;
		output.interp3.xyzw = input.texCoord0;
		output.interp4.xyz = input.viewDirectionWS;
		#if defined(LIGHTMAP_ON)
		output.interp5.xy = input.lightmapUV;
		#endif
		#if !defined(LIGHTMAP_ON)
		output.interp6.xyz = input.sh;
		#endif
		output.interp7.xyzw = input.fogFactorAndVertexLight;
		output.interp8.xyzw = input.shadowCoord;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		output.normalWS = input.interp1.xyz;
		output.tangentWS = input.interp2.xyzw;
		output.texCoord0 = input.interp3.xyzw;
		output.viewDirectionWS = input.interp4.xyz;
		#if defined(LIGHTMAP_ON)
		output.lightmapUV = input.interp5.xy;
		#endif
		#if !defined(LIGHTMAP_ON)
		output.sh = input.interp6.xyz;
		#endif
		output.fogFactorAndVertexLight = input.interp7.xyzw;
		output.shadowCoord = input.interp8.xyzw;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
	Out = A + B;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
	Out = A * B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float3 BaseColor;
	float3 NormalTS;
	float3 Emission;
	float Metallic;
	float Smoothness;
	float Occlusion;
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	float4 _Property_2165fc3283a241e482d281330b4086e1_Out_0 = Color_afbdcc2f0d024fe08de994e91925be3b;
	UnityTexture2D _Property_2ef9f30408844319bfcc4bea45316d28_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
	float4 _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2ef9f30408844319bfcc4bea45316d28_Out_0.tex, _Property_2ef9f30408844319bfcc4bea45316d28_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_R_4 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.r;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_G_5 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.g;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_B_6 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.b;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_A_7 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.a;
	float4 _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2;
	Unity_Add_float4(_Property_2165fc3283a241e482d281330b4086e1_Out_0, _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0, _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2);
	UnityTexture2D _Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
	float4 _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0 = SAMPLE_TEXTURE2D(_Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0.tex, _Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0.samplerstate, IN.uv0.xy);
	_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0);
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_R_4 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.r;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_G_5 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.g;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_B_6 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.b;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_A_7 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.a;
	UnityTexture2D _Property_4582d80c094d4b188e907208ff0a494a_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_933df24ac27a484f9b6e59168017b62d);
	float4 _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4582d80c094d4b188e907208ff0a494a_Out_0.tex, _Property_4582d80c094d4b188e907208ff0a494a_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_R_4 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.r;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_G_5 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.g;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_B_6 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.b;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_A_7 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.a;
	float4 _Property_1a42c57c811b4b5bb19f39dd500bdbc9_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_a5bb6ace4e044cee85204dac7bccd927) : Color_a5bb6ace4e044cee85204dac7bccd927;
	float4 _Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2;
	Unity_Multiply_float(_SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0, _Property_1a42c57c811b4b5bb19f39dd500bdbc9_Out_0, _Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2);
	float _Property_81611d5549fd4509b86de5c2aa9f6f4d_Out_0 = Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
	UnityTexture2D _Property_4af1499beb0a4526bb400fbc58d1b006_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_b604a5e590b648cb846f3bfebb35331c);
	float4 _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4af1499beb0a4526bb400fbc58d1b006_Out_0.tex, _Property_4af1499beb0a4526bb400fbc58d1b006_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_f453c42eef784618af6744ea9794da15_R_4 = _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0.r;
	float _SampleTexture2D_f453c42eef784618af6744ea9794da15_G_5 = _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0.g;
	float _SampleTexture2D_f453c42eef784618af6744ea9794da15_B_6 = _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0.b;
	float _SampleTexture2D_f453c42eef784618af6744ea9794da15_A_7 = _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0.a;
	float4 _Multiply_16bc27119a6845e99c0147374e4c55c8_Out_2;
	Unity_Multiply_float((_Property_81611d5549fd4509b86de5c2aa9f6f4d_Out_0.xxxx), _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0, _Multiply_16bc27119a6845e99c0147374e4c55c8_Out_2);
	float _Property_921137aef1624dea8e57841a867425e2_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float _Multiply_e9d1907defde412bb1429f82e8510226_Out_2;
	Unity_Multiply_float(_Property_921137aef1624dea8e57841a867425e2_Out_0, _SampleTexture2D_f453c42eef784618af6744ea9794da15_A_7, _Multiply_e9d1907defde412bb1429f82e8510226_Out_2);
	UnityTexture2D _Property_d2249d799da3488d874fcdb0e76ba2f5_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
	float4 _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d2249d799da3488d874fcdb0e76ba2f5_Out_0.tex, _Property_d2249d799da3488d874fcdb0e76ba2f5_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_R_4 = _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0.r;
	float _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_G_5 = _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0.g;
	float _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_B_6 = _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0.b;
	float _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_A_7 = _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0.a;
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.BaseColor = (_Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2.xyz);
	surface.NormalTS = (_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.xyz);
	surface.Emission = (_Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2.xyz);
	surface.Metallic = (_Multiply_16bc27119a6845e99c0147374e4c55c8_Out_2).x;
	surface.Smoothness = _Multiply_e9d1907defde412bb1429f82e8510226_Out_2;
	surface.Occlusion = (_SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0).x;
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



	output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
	output.uv0 = input.texCoord0;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRGBufferPass.hlsl"

	ENDHLSL
}
Pass
{
	Name "ShadowCaster"
	Tags
	{
		"LightMode" = "ShadowCaster"
	}

		// Render State
		Cull Back
	Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
	ZTest LEqual
	ZWrite On
	ColorMask 0

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 4.5
	#pragma exclude_renderers gles gles3 glcore
	#pragma multi_compile_instancing
	#pragma multi_compile _ DOTS_INSTANCING_ON
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		// PassKeywords: <None>
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define VARYINGS_NEED_POSITION_WS
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_SHADOWCASTER
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 WorldSpacePosition;
		float4 ScreenPosition;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

	ENDHLSL
}
Pass
{
	Name "DepthOnly"
	Tags
	{
		"LightMode" = "DepthOnly"
	}

		// Render State
		Cull Back
	Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
	ZTest LEqual
	ZWrite On
	ColorMask 0

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 4.5
	#pragma exclude_renderers gles gles3 glcore
	#pragma multi_compile_instancing
	#pragma multi_compile _ DOTS_INSTANCING_ON
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		// PassKeywords: <None>
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define VARYINGS_NEED_POSITION_WS
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_DEPTHONLY
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 WorldSpacePosition;
		float4 ScreenPosition;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

	ENDHLSL
}
Pass
{
	Name "DepthNormals"
	Tags
	{
		"LightMode" = "DepthNormals"
	}

		// Render State
		Cull Back
	Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
	ZTest LEqual
	ZWrite On

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 4.5
	#pragma exclude_renderers gles gles3 glcore
	#pragma multi_compile_instancing
	#pragma multi_compile _ DOTS_INSTANCING_ON
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		// PassKeywords: <None>
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define ATTRIBUTES_NEED_TEXCOORD1
		#define VARYINGS_NEED_POSITION_WS
		#define VARYINGS_NEED_NORMAL_WS
		#define VARYINGS_NEED_TANGENT_WS
		#define VARYINGS_NEED_TEXCOORD0
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		float4 uv0 : TEXCOORD0;
		float4 uv1 : TEXCOORD1;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		float3 normalWS;
		float4 tangentWS;
		float4 texCoord0;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 TangentSpaceNormal;
		float3 WorldSpacePosition;
		float4 ScreenPosition;
		float4 uv0;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		float3 interp1 : TEXCOORD1;
		float4 interp2 : TEXCOORD2;
		float4 interp3 : TEXCOORD3;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		output.interp1.xyz = input.normalWS;
		output.interp2.xyzw = input.tangentWS;
		output.interp3.xyzw = input.texCoord0;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		output.normalWS = input.interp1.xyz;
		output.tangentWS = input.interp2.xyzw;
		output.texCoord0 = input.interp3.xyzw;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float3 NormalTS;
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	UnityTexture2D _Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
	float4 _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0 = SAMPLE_TEXTURE2D(_Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0.tex, _Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0.samplerstate, IN.uv0.xy);
	_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0);
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_R_4 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.r;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_G_5 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.g;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_B_6 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.b;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_A_7 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.a;
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.NormalTS = (_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.xyz);
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



	output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
	output.uv0 = input.texCoord0;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

	ENDHLSL
}
Pass
{
	Name "Meta"
	Tags
	{
		"LightMode" = "Meta"
	}

		// Render State
		Cull Off

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 4.5
	#pragma exclude_renderers gles gles3 glcore
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define ATTRIBUTES_NEED_TEXCOORD1
		#define ATTRIBUTES_NEED_TEXCOORD2
		#define VARYINGS_NEED_POSITION_WS
		#define VARYINGS_NEED_TEXCOORD0
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_META
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		float4 uv0 : TEXCOORD0;
		float4 uv1 : TEXCOORD1;
		float4 uv2 : TEXCOORD2;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		float4 texCoord0;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 WorldSpacePosition;
		float4 ScreenPosition;
		float4 uv0;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		float4 interp1 : TEXCOORD1;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		output.interp1.xyzw = input.texCoord0;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		output.texCoord0 = input.interp1.xyzw;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
	Out = A + B;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
	Out = A * B;
}

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float3 BaseColor;
	float3 Emission;
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	float4 _Property_2165fc3283a241e482d281330b4086e1_Out_0 = Color_afbdcc2f0d024fe08de994e91925be3b;
	UnityTexture2D _Property_2ef9f30408844319bfcc4bea45316d28_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
	float4 _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2ef9f30408844319bfcc4bea45316d28_Out_0.tex, _Property_2ef9f30408844319bfcc4bea45316d28_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_R_4 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.r;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_G_5 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.g;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_B_6 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.b;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_A_7 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.a;
	float4 _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2;
	Unity_Add_float4(_Property_2165fc3283a241e482d281330b4086e1_Out_0, _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0, _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2);
	UnityTexture2D _Property_4582d80c094d4b188e907208ff0a494a_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_933df24ac27a484f9b6e59168017b62d);
	float4 _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4582d80c094d4b188e907208ff0a494a_Out_0.tex, _Property_4582d80c094d4b188e907208ff0a494a_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_R_4 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.r;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_G_5 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.g;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_B_6 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.b;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_A_7 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.a;
	float4 _Property_1a42c57c811b4b5bb19f39dd500bdbc9_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_a5bb6ace4e044cee85204dac7bccd927) : Color_a5bb6ace4e044cee85204dac7bccd927;
	float4 _Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2;
	Unity_Multiply_float(_SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0, _Property_1a42c57c811b4b5bb19f39dd500bdbc9_Out_0, _Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2);
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.BaseColor = (_Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2.xyz);
	surface.Emission = (_Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2.xyz);
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
	output.uv0 = input.texCoord0;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

	ENDHLSL
}
Pass
{
		// Name: <None>
		Tags
		{
			"LightMode" = "Universal2D"
		}

		// Render State
		Cull Back
	Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
	ZTest LEqual
	ZWrite Off

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 4.5
	#pragma exclude_renderers gles gles3 glcore
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		// PassKeywords: <None>
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define VARYINGS_NEED_POSITION_WS
		#define VARYINGS_NEED_TEXCOORD0
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_2D
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		float4 uv0 : TEXCOORD0;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		float4 texCoord0;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 WorldSpacePosition;
		float4 ScreenPosition;
		float4 uv0;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		float4 interp1 : TEXCOORD1;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		output.interp1.xyzw = input.texCoord0;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		output.texCoord0 = input.interp1.xyzw;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
	Out = A + B;
}

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float3 BaseColor;
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	float4 _Property_2165fc3283a241e482d281330b4086e1_Out_0 = Color_afbdcc2f0d024fe08de994e91925be3b;
	UnityTexture2D _Property_2ef9f30408844319bfcc4bea45316d28_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
	float4 _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2ef9f30408844319bfcc4bea45316d28_Out_0.tex, _Property_2ef9f30408844319bfcc4bea45316d28_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_R_4 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.r;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_G_5 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.g;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_B_6 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.b;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_A_7 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.a;
	float4 _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2;
	Unity_Add_float4(_Property_2165fc3283a241e482d281330b4086e1_Out_0, _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0, _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2);
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.BaseColor = (_Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2.xyz);
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
	output.uv0 = input.texCoord0;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

	ENDHLSL
}
	}
		SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType" = "Transparent"
			"UniversalMaterialType" = "Lit"
			"Queue" = "Transparent"
		}
		Pass
		{
			Name "Universal Forward"
			Tags
			{
				"LightMode" = "UniversalForward"
			}

		// Render State
		Cull Back
	Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
	ZTest LEqual
	ZWrite Off

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 2.0
	#pragma only_renderers gles gles3 glcore d3d11
	#pragma multi_compile_instancing
	#pragma multi_compile_fog
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
	#pragma multi_compile _ LIGHTMAP_ON
	#pragma multi_compile _ DIRLIGHTMAP_COMBINED
	#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
	#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
	#pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
	#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
	#pragma multi_compile _ _SHADOWS_SOFT
	#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
	#pragma multi_compile _ SHADOWS_SHADOWMASK
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define ATTRIBUTES_NEED_TEXCOORD1
		#define VARYINGS_NEED_POSITION_WS
		#define VARYINGS_NEED_NORMAL_WS
		#define VARYINGS_NEED_TANGENT_WS
		#define VARYINGS_NEED_TEXCOORD0
		#define VARYINGS_NEED_VIEWDIRECTION_WS
		#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_FORWARD
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		float4 uv0 : TEXCOORD0;
		float4 uv1 : TEXCOORD1;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		float3 normalWS;
		float4 tangentWS;
		float4 texCoord0;
		float3 viewDirectionWS;
		#if defined(LIGHTMAP_ON)
		float2 lightmapUV;
		#endif
		#if !defined(LIGHTMAP_ON)
		float3 sh;
		#endif
		float4 fogFactorAndVertexLight;
		float4 shadowCoord;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 TangentSpaceNormal;
		float3 WorldSpacePosition;
		float4 ScreenPosition;
		float4 uv0;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		float3 interp1 : TEXCOORD1;
		float4 interp2 : TEXCOORD2;
		float4 interp3 : TEXCOORD3;
		float3 interp4 : TEXCOORD4;
		#if defined(LIGHTMAP_ON)
		float2 interp5 : TEXCOORD5;
		#endif
		#if !defined(LIGHTMAP_ON)
		float3 interp6 : TEXCOORD6;
		#endif
		float4 interp7 : TEXCOORD7;
		float4 interp8 : TEXCOORD8;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		output.interp1.xyz = input.normalWS;
		output.interp2.xyzw = input.tangentWS;
		output.interp3.xyzw = input.texCoord0;
		output.interp4.xyz = input.viewDirectionWS;
		#if defined(LIGHTMAP_ON)
		output.interp5.xy = input.lightmapUV;
		#endif
		#if !defined(LIGHTMAP_ON)
		output.interp6.xyz = input.sh;
		#endif
		output.interp7.xyzw = input.fogFactorAndVertexLight;
		output.interp8.xyzw = input.shadowCoord;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		output.normalWS = input.interp1.xyz;
		output.tangentWS = input.interp2.xyzw;
		output.texCoord0 = input.interp3.xyzw;
		output.viewDirectionWS = input.interp4.xyz;
		#if defined(LIGHTMAP_ON)
		output.lightmapUV = input.interp5.xy;
		#endif
		#if !defined(LIGHTMAP_ON)
		output.sh = input.interp6.xyz;
		#endif
		output.fogFactorAndVertexLight = input.interp7.xyzw;
		output.shadowCoord = input.interp8.xyzw;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
	Out = A + B;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
	Out = A * B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float3 BaseColor;
	float3 NormalTS;
	float3 Emission;
	float Metallic;
	float Smoothness;
	float Occlusion;
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	float4 _Property_2165fc3283a241e482d281330b4086e1_Out_0 = Color_afbdcc2f0d024fe08de994e91925be3b;
	UnityTexture2D _Property_2ef9f30408844319bfcc4bea45316d28_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
	float4 _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2ef9f30408844319bfcc4bea45316d28_Out_0.tex, _Property_2ef9f30408844319bfcc4bea45316d28_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_R_4 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.r;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_G_5 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.g;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_B_6 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.b;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_A_7 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.a;
	float4 _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2;
	Unity_Add_float4(_Property_2165fc3283a241e482d281330b4086e1_Out_0, _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0, _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2);
	UnityTexture2D _Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
	float4 _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0 = SAMPLE_TEXTURE2D(_Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0.tex, _Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0.samplerstate, IN.uv0.xy);
	_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0);
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_R_4 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.r;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_G_5 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.g;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_B_6 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.b;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_A_7 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.a;
	UnityTexture2D _Property_4582d80c094d4b188e907208ff0a494a_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_933df24ac27a484f9b6e59168017b62d);
	float4 _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4582d80c094d4b188e907208ff0a494a_Out_0.tex, _Property_4582d80c094d4b188e907208ff0a494a_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_R_4 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.r;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_G_5 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.g;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_B_6 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.b;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_A_7 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.a;
	float4 _Property_1a42c57c811b4b5bb19f39dd500bdbc9_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_a5bb6ace4e044cee85204dac7bccd927) : Color_a5bb6ace4e044cee85204dac7bccd927;
	float4 _Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2;
	Unity_Multiply_float(_SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0, _Property_1a42c57c811b4b5bb19f39dd500bdbc9_Out_0, _Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2);
	float _Property_81611d5549fd4509b86de5c2aa9f6f4d_Out_0 = Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
	UnityTexture2D _Property_4af1499beb0a4526bb400fbc58d1b006_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_b604a5e590b648cb846f3bfebb35331c);
	float4 _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4af1499beb0a4526bb400fbc58d1b006_Out_0.tex, _Property_4af1499beb0a4526bb400fbc58d1b006_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_f453c42eef784618af6744ea9794da15_R_4 = _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0.r;
	float _SampleTexture2D_f453c42eef784618af6744ea9794da15_G_5 = _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0.g;
	float _SampleTexture2D_f453c42eef784618af6744ea9794da15_B_6 = _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0.b;
	float _SampleTexture2D_f453c42eef784618af6744ea9794da15_A_7 = _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0.a;
	float4 _Multiply_16bc27119a6845e99c0147374e4c55c8_Out_2;
	Unity_Multiply_float((_Property_81611d5549fd4509b86de5c2aa9f6f4d_Out_0.xxxx), _SampleTexture2D_f453c42eef784618af6744ea9794da15_RGBA_0, _Multiply_16bc27119a6845e99c0147374e4c55c8_Out_2);
	float _Property_921137aef1624dea8e57841a867425e2_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float _Multiply_e9d1907defde412bb1429f82e8510226_Out_2;
	Unity_Multiply_float(_Property_921137aef1624dea8e57841a867425e2_Out_0, _SampleTexture2D_f453c42eef784618af6744ea9794da15_A_7, _Multiply_e9d1907defde412bb1429f82e8510226_Out_2);
	UnityTexture2D _Property_d2249d799da3488d874fcdb0e76ba2f5_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
	float4 _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d2249d799da3488d874fcdb0e76ba2f5_Out_0.tex, _Property_d2249d799da3488d874fcdb0e76ba2f5_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_R_4 = _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0.r;
	float _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_G_5 = _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0.g;
	float _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_B_6 = _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0.b;
	float _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_A_7 = _SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0.a;
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.BaseColor = (_Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2.xyz);
	surface.NormalTS = (_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.xyz);
	surface.Emission = (_Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2.xyz);
	surface.Metallic = (_Multiply_16bc27119a6845e99c0147374e4c55c8_Out_2).x;
	surface.Smoothness = _Multiply_e9d1907defde412bb1429f82e8510226_Out_2;
	surface.Occlusion = (_SampleTexture2D_4bd563ed556c435198340d967e6e27d3_RGBA_0).x;
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



	output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
	output.uv0 = input.texCoord0;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

	ENDHLSL
}
Pass
{
	Name "ShadowCaster"
	Tags
	{
		"LightMode" = "ShadowCaster"
	}

		// Render State
		Cull Back
	Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
	ZTest LEqual
	ZWrite On
	ColorMask 0

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 2.0
	#pragma only_renderers gles gles3 glcore d3d11
	#pragma multi_compile_instancing
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		// PassKeywords: <None>
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define VARYINGS_NEED_POSITION_WS
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_SHADOWCASTER
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 WorldSpacePosition;
		float4 ScreenPosition;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

	ENDHLSL
}
Pass
{
	Name "DepthOnly"
	Tags
	{
		"LightMode" = "DepthOnly"
	}

		// Render State
		Cull Back
	Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
	ZTest LEqual
	ZWrite On
	ColorMask 0

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 2.0
	#pragma only_renderers gles gles3 glcore d3d11
	#pragma multi_compile_instancing
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		// PassKeywords: <None>
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define VARYINGS_NEED_POSITION_WS
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_DEPTHONLY
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 WorldSpacePosition;
		float4 ScreenPosition;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

	ENDHLSL
}
Pass
{
	Name "DepthNormals"
	Tags
	{
		"LightMode" = "DepthNormals"
	}

		// Render State
		Cull Back
	Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
	ZTest LEqual
	ZWrite On

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 2.0
	#pragma only_renderers gles gles3 glcore d3d11
	#pragma multi_compile_instancing
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		// PassKeywords: <None>
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define ATTRIBUTES_NEED_TEXCOORD1
		#define VARYINGS_NEED_POSITION_WS
		#define VARYINGS_NEED_NORMAL_WS
		#define VARYINGS_NEED_TANGENT_WS
		#define VARYINGS_NEED_TEXCOORD0
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		float4 uv0 : TEXCOORD0;
		float4 uv1 : TEXCOORD1;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		float3 normalWS;
		float4 tangentWS;
		float4 texCoord0;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 TangentSpaceNormal;
		float3 WorldSpacePosition;
		float4 ScreenPosition;
		float4 uv0;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		float3 interp1 : TEXCOORD1;
		float4 interp2 : TEXCOORD2;
		float4 interp3 : TEXCOORD3;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		output.interp1.xyz = input.normalWS;
		output.interp2.xyzw = input.tangentWS;
		output.interp3.xyzw = input.texCoord0;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		output.normalWS = input.interp1.xyz;
		output.tangentWS = input.interp2.xyzw;
		output.texCoord0 = input.interp3.xyzw;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float3 NormalTS;
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	UnityTexture2D _Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
	float4 _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0 = SAMPLE_TEXTURE2D(_Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0.tex, _Property_09ac14ab723d4d1b9cc5addcdfb2e473_Out_0.samplerstate, IN.uv0.xy);
	_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.rgb = UnpackNormal(_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0);
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_R_4 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.r;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_G_5 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.g;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_B_6 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.b;
	float _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_A_7 = _SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.a;
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.NormalTS = (_SampleTexture2D_cea7c489f66347d388922075bf1a2f92_RGBA_0.xyz);
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);



	output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
	output.uv0 = input.texCoord0;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

	ENDHLSL
}
Pass
{
	Name "Meta"
	Tags
	{
		"LightMode" = "Meta"
	}

		// Render State
		Cull Off

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 2.0
	#pragma only_renderers gles gles3 glcore d3d11
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define ATTRIBUTES_NEED_TEXCOORD1
		#define ATTRIBUTES_NEED_TEXCOORD2
		#define VARYINGS_NEED_POSITION_WS
		#define VARYINGS_NEED_TEXCOORD0
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_META
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		float4 uv0 : TEXCOORD0;
		float4 uv1 : TEXCOORD1;
		float4 uv2 : TEXCOORD2;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		float4 texCoord0;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 WorldSpacePosition;
		float4 ScreenPosition;
		float4 uv0;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		float4 interp1 : TEXCOORD1;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		output.interp1.xyzw = input.texCoord0;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		output.texCoord0 = input.interp1.xyzw;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
	Out = A + B;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
	Out = A * B;
}

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float3 BaseColor;
	float3 Emission;
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	float4 _Property_2165fc3283a241e482d281330b4086e1_Out_0 = Color_afbdcc2f0d024fe08de994e91925be3b;
	UnityTexture2D _Property_2ef9f30408844319bfcc4bea45316d28_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
	float4 _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2ef9f30408844319bfcc4bea45316d28_Out_0.tex, _Property_2ef9f30408844319bfcc4bea45316d28_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_R_4 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.r;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_G_5 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.g;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_B_6 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.b;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_A_7 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.a;
	float4 _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2;
	Unity_Add_float4(_Property_2165fc3283a241e482d281330b4086e1_Out_0, _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0, _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2);
	UnityTexture2D _Property_4582d80c094d4b188e907208ff0a494a_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_933df24ac27a484f9b6e59168017b62d);
	float4 _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4582d80c094d4b188e907208ff0a494a_Out_0.tex, _Property_4582d80c094d4b188e907208ff0a494a_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_R_4 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.r;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_G_5 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.g;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_B_6 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.b;
	float _SampleTexture2D_629d5d74f5574b398acde802bef39e11_A_7 = _SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0.a;
	float4 _Property_1a42c57c811b4b5bb19f39dd500bdbc9_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_a5bb6ace4e044cee85204dac7bccd927) : Color_a5bb6ace4e044cee85204dac7bccd927;
	float4 _Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2;
	Unity_Multiply_float(_SampleTexture2D_629d5d74f5574b398acde802bef39e11_RGBA_0, _Property_1a42c57c811b4b5bb19f39dd500bdbc9_Out_0, _Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2);
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.BaseColor = (_Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2.xyz);
	surface.Emission = (_Multiply_72a993b3030e40a5ae4ac24938fb3ad8_Out_2.xyz);
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
	output.uv0 = input.texCoord0;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

	ENDHLSL
}
Pass
{
		// Name: <None>
		Tags
		{
			"LightMode" = "Universal2D"
		}

		// Render State
		Cull Back
	Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
	ZTest LEqual
	ZWrite Off

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		HLSLPROGRAM

		// Pragmas
		#pragma target 2.0
	#pragma only_renderers gles gles3 glcore d3d11
	#pragma multi_compile_instancing
	#pragma vertex vert
	#pragma fragment frag

		// DotsInstancingOptions: <None>
		// HybridV1InjectedBuiltinProperties: <None>

		// Keywords
		// PassKeywords: <None>
		// GraphKeywords: <None>

		// Defines
		#define _SURFACE_TYPE_TRANSPARENT 1
		#define _AlphaClip 1
		#define _NORMALMAP 1
		#define _NORMAL_DROPOFF_TS 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define VARYINGS_NEED_POSITION_WS
		#define VARYINGS_NEED_TEXCOORD0
		#define FEATURES_GRAPH_VERTEX
		/* WARNING: $splice Could not find named fragment 'PassInstancing' */
		#define SHADERPASS SHADERPASS_2D
		/* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Structs and Packing

		struct Attributes
	{
		float3 positionOS : POSITION;
		float3 normalOS : NORMAL;
		float4 tangentOS : TANGENT;
		float4 uv0 : TEXCOORD0;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : INSTANCEID_SEMANTIC;
		#endif
	};
	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float3 positionWS;
		float4 texCoord0;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};
	struct SurfaceDescriptionInputs
	{
		float3 WorldSpacePosition;
		float4 ScreenPosition;
		float4 uv0;
	};
	struct VertexDescriptionInputs
	{
		float3 ObjectSpaceNormal;
		float3 ObjectSpaceTangent;
		float3 ObjectSpacePosition;
	};
	struct PackedVaryings
	{
		float4 positionCS : SV_POSITION;
		float3 interp0 : TEXCOORD0;
		float4 interp1 : TEXCOORD1;
		#if UNITY_ANY_INSTANCING_ENABLED
		uint instanceID : CUSTOM_INSTANCE_ID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
		#endif
	};

		PackedVaryings PackVaryings(Varyings input)
	{
		PackedVaryings output;
		output.positionCS = input.positionCS;
		output.interp0.xyz = input.positionWS;
		output.interp1.xyzw = input.texCoord0;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}
	Varyings UnpackVaryings(PackedVaryings input)
	{
		Varyings output;
		output.positionCS = input.positionCS;
		output.positionWS = input.interp0.xyz;
		output.texCoord0 = input.interp1.xyzw;
		#if UNITY_ANY_INSTANCING_ENABLED
		output.instanceID = input.instanceID;
		#endif
		#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
		output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
		#endif
		#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
		output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
		#endif
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		output.cullFace = input.cullFace;
		#endif
		return output;
	}

	// --------------------------------------------------
	// Graph

	// Graph Properties
	CBUFFER_START(UnityPerMaterial)
float4 Color_afbdcc2f0d024fe08de994e91925be3b;
float Vector1_1;
float2 _PlayerPosition;
float _Size;
float Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
float4 Texture2D_3aaa6060906e457383ff3ad5809e2d06_TexelSize;
float4 Texture2D_b604a5e590b648cb846f3bfebb35331c_TexelSize;
float4 Texture2D_b0650479ef3a4d8e95897f48d47256e8_TexelSize;
float4 Texture2D_bbb0f0de226b483e97dfd4fac79cff11_TexelSize;
float4 Texture2D_933df24ac27a484f9b6e59168017b62d_TexelSize;
float4 Color_a5bb6ace4e044cee85204dac7bccd927;
float Vector1_f7916f3f7aef4c0aaaed9bd20c6a242d;
float Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
SAMPLER(samplerTexture2D_3aaa6060906e457383ff3ad5809e2d06);
TEXTURE2D(Texture2D_b604a5e590b648cb846f3bfebb35331c);
SAMPLER(samplerTexture2D_b604a5e590b648cb846f3bfebb35331c);
TEXTURE2D(Texture2D_b0650479ef3a4d8e95897f48d47256e8);
SAMPLER(samplerTexture2D_b0650479ef3a4d8e95897f48d47256e8);
TEXTURE2D(Texture2D_bbb0f0de226b483e97dfd4fac79cff11);
SAMPLER(samplerTexture2D_bbb0f0de226b483e97dfd4fac79cff11);
TEXTURE2D(Texture2D_933df24ac27a484f9b6e59168017b62d);
SAMPLER(samplerTexture2D_933df24ac27a484f9b6e59168017b62d);

// Graph Functions

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
	Out = A + B;
}

void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
{
	Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Add_float2(float2 A, float2 B, out float2 Out)
{
	Out = A + B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
	Out = UV * Tiling + Offset;
}

void Unity_Multiply_float(float2 A, float2 B, out float2 Out)
{
	Out = A * B;
}

void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
{
	Out = A - B;
}

void Unity_Divide_float(float A, float B, out float Out)
{
	Out = A / B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
	Out = A * B;
}

void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
{
	Out = A / B;
}

void Unity_Length_float2(float2 In, out float Out)
{
	Out = length(In);
}

void Unity_OneMinus_float(float In, out float Out)
{
	Out = 1 - In;
}

void Unity_Saturate_float(float In, out float Out)
{
	Out = saturate(In);
}

void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
{
	Out = smoothstep(Edge1, Edge2, In);
}

// Graph Vertex
struct VertexDescription
{
	float3 Position;
	float3 Normal;
	float3 Tangent;
};

VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
{
	VertexDescription description = (VertexDescription)0;
	description.Position = IN.ObjectSpacePosition;
	description.Normal = IN.ObjectSpaceNormal;
	description.Tangent = IN.ObjectSpaceTangent;
	return description;
}

// Graph Pixel
struct SurfaceDescription
{
	float3 BaseColor;
	float Alpha;
	float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
	SurfaceDescription surface = (SurfaceDescription)0;
	float4 _Property_2165fc3283a241e482d281330b4086e1_Out_0 = Color_afbdcc2f0d024fe08de994e91925be3b;
	UnityTexture2D _Property_2ef9f30408844319bfcc4bea45316d28_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_3aaa6060906e457383ff3ad5809e2d06);
	float4 _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2ef9f30408844319bfcc4bea45316d28_Out_0.tex, _Property_2ef9f30408844319bfcc4bea45316d28_Out_0.samplerstate, IN.uv0.xy);
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_R_4 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.r;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_G_5 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.g;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_B_6 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.b;
	float _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_A_7 = _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0.a;
	float4 _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2;
	Unity_Add_float4(_Property_2165fc3283a241e482d281330b4086e1_Out_0, _SampleTexture2D_1899c86c69684c73abb1ce5a5a443162_RGBA_0, _Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2);
	float _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0 = Vector1_2ed4051ddd1c4dc7ab4d818f711f3c42;
	float4 _ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
	float2 _Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0 = _PlayerPosition;
	float2 _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3;
	Unity_Remap_float2(_Property_e7c7ff700d1c4b94b2c5f002f5d13b41_Out_0, float2 (0, 1), float2 (0.5, -1.5), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3);
	float2 _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2;
	Unity_Add_float2((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), _Remap_9729c0ca36494a0899d28fae1a2b69f2_Out_3, _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2);
	float2 _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3;
	Unity_TilingAndOffset_float((_ScreenPosition_553f60ecef4e406fb0280604c4f862f2_Out_0.xy), float2 (1, 1), _Add_29c0738b03184ac08c3d209f45ea0bac_Out_2, _TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3);
	float2 _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2;
	Unity_Multiply_float(_TilingAndOffset_2ec9e8d404284434aa5dc3b5405ffef6_Out_3, float2(2, 2), _Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2);
	float2 _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2;
	Unity_Subtract_float2(_Multiply_ce59bd37ccc745d8b1cfb633f3a32769_Out_2, float2(1, 1), _Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2);
	float _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2;
	Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2);
	float _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0 = _Size;
	float _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2;
	Unity_Multiply_float(_Divide_d7340b709b6b4daaab9b423fea9148ff_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0, _Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2);
	float2 _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0 = float2(_Multiply_239e6cbd4e854be884a4f50016bdd1c1_Out_2, _Property_9dbb542db1564270ba5e8b79eecc3209_Out_0);
	float2 _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2;
	Unity_Divide_float2(_Subtract_0df4f5ad6315412e9ebbbdcd07e40489_Out_2, _Vector2_ffed95861e1946e0a0acd8606dddcaf4_Out_0, _Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2);
	float _Length_097434223074471ab9b5b2b06839d6ee_Out_1;
	Unity_Length_float2(_Divide_bc1783f979b74a3eaac2aaa7d013c553_Out_2, _Length_097434223074471ab9b5b2b06839d6ee_Out_1);
	float _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1;
	Unity_OneMinus_float(_Length_097434223074471ab9b5b2b06839d6ee_Out_1, _OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1);
	float _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1;
	Unity_Saturate_float(_OneMinus_e3ba48cd1a60439ca345efebdbcb02ef_Out_1, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1);
	float _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3;
	Unity_Smoothstep_float(0, _Property_9bce7a6c5b33441fbbfc3f53a8066921_Out_0, _Saturate_ef53ab75c13b4965bcfdf15188ccb67c_Out_1, _Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3);
	float _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0 = Vector1_ae10dd6a890d4bb6bcc68363347ade9a;
	float _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2;
	Unity_Multiply_float(_Smoothstep_9a6ab5f1e5c1499b9f6f4b4fb92b478c_Out_3, _Property_efbc89064ec2494d80897c684ab3a5fd_Out_0, _Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2);
	float _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	Unity_OneMinus_float(_Multiply_1d6448b334a04c64a59f7f98c014a723_Out_2, _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1);
	surface.BaseColor = (_Add_b24c16c0a57d4d10ba9c02e948b52369_Out_2.xyz);
	surface.Alpha = _OneMinus_d8594215b7e34f429c8ce6370333de6d_Out_1;
	surface.AlphaClipThreshold = 0;
	return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
	VertexDescriptionInputs output;
	ZERO_INITIALIZE(VertexDescriptionInputs, output);

	output.ObjectSpaceNormal = input.normalOS;
	output.ObjectSpaceTangent = input.tangentOS.xyz;
	output.ObjectSpacePosition = input.positionOS;

	return output;
}
	SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
	SurfaceDescriptionInputs output;
	ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





	output.WorldSpacePosition = input.positionWS;
	output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
	output.uv0 = input.texCoord0;
#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
#else
#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
#endif
#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

	return output;
}

	// --------------------------------------------------
	// Main

	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

	ENDHLSL
}
	}
		CustomEditor "ShaderGraph.PBRMasterGUI"
		FallBack "Hidden/Shader Graph/FallbackError"
}