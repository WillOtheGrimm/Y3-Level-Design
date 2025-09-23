// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

/*
MIT License

Copyright(c) 2017 Raphael Tetreault

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files(the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions :

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

Shader "Custom/GreyboxUnit" {
	Properties{
		_ColorA("ColorA", Color) = (1,1,1,1)
		_ColorB("ColorB", Color) = (0,0,0,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_UnitSize("Grid Unit", float) = 1.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
		float3 worldPos;
	};

	fixed4 _ColorA;
	fixed4 _ColorB;
	half _Glossiness;
	half _Metallic;
	float _UnitSize;

	// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
	// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
	// #pragma instancing_options assumeuniformscaling
	UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
	{
		// Manually wrap
		float3 pos = fmod(IN.worldPos, (float3(_UnitSize, _UnitSize, _UnitSize) * 2));

		// Prevent negative values from breaking wrap
		// Mirror volumes if negative (prevent adjacent cubes along axis lines)
		bool x = abs(pos.x) > _UnitSize ^ pos.x > 0;
		bool y = abs(pos.y) > _UnitSize ^ pos.y > 0;
		bool z = abs(pos.z) > _UnitSize ^ pos.z > 0;

		fixed4 c = (x^y^z) ? _ColorA : _ColorB;

		o.Albedo = c.rgb;
		// Metallic and smoothness come from slider variables
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}