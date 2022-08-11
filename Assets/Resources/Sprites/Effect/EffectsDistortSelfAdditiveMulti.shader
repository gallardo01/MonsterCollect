Shader "Effects/Distortion/DistortSelfAdditiveMulti" {


	/*
	this shader uses 1 texture to distort 2 other textures added together
	and has a mask that can mask either the alpha, or the distortion
	there is a main color tint
	the brightness is additive
	the bias controls the amount of distortion, it multiplies the distance to distort
	both textures pan, start with 5 when tuning
	(C) Play Studios 2015
	chris rogers 7/2015 edited gbeckman 1/2017

	*/
	Properties{
		[Header(Main Attributes)]
		_Color("Main RGB", Color) = (0.5, 0.5, 0.5, 0.5)
		_Brightness("Brightness", float) = 0
		_Alpha("Alpha", Range(0,3)) = 1
		[Header(________________________________________________________________________________________________________)]
		[Space(5)]

		////main color texture
		[Header(Diffuse Texture Attributes)]
		_Tex1("Diffuse 1 (RGB)", 2D) = "white" {}
		_Pan1("Pan1", Vector) = (0,0,0,0)
		_Rot1("Tex1 Rotation Offset", Float) = 0
		_Rot1Pan("Tex1 Rotation Pan", Float) = 0

		////color noise mult texture
		[Space(10)]
		_Tex1b("Diffuse 2 (RGB)", 2D) = "white" {}
		_Pan1b("Pan1b", Vector) = (0,0,0,0)
		_Rot2("Tex2 Rotation Offset", Float) = 0
		_Rot2Pan("Tex2 Rotation Pan", Float) = 0

		[Space(15)]

		_Tex1bAmount("Texture Blend", Range(0,1)) = 1
		_Tex1Bright("Texture 1 Add", float) = 0
		_Tex1bBright("Texture 2 Add", float) = 0

		[Header(________________________________________________________________________________________________________)]

		[Space(5)]
		////Distort texture 1
		[Header(Distortion Texture Attributes)]

		_DisTex("Distort (RGB)", 2D) = "white" {}
		_DisBias("Bias (amount)", float) = 0
		[MaterialToggle] HorizBias("Separate Bias Direction", Float) = 0
		_DisBiasY("Bias Y (amount)", float) = 0
		_DisPan("Distort Pan", Vector) = (0,0,0,0)

		[Header(________________________________________________________________________________________________________)]

		[Space(5)]
		_MaskTex("Mask (A)", 2D) = "white" {}
		_MaskPan("Mask Pan", Vector) = (0,0,0,0)
		_RotMask("Mask Rotation Offset", Float) = 0
		_RotMaskPan("Mask Rotation Pan", Float) = 0
		[MaterialToggle] MaskDistort("Mask Distortion Instead of Alpha", Float) = 0


		[Header(________________________________________________________________________________________________________)]

		[Space(5)]
		[MaterialToggle]  UVSet2("Use UV set 2", Float) = 0

		[Header(________________________________________________________________________________________________________)]
		[Space(5)]
		_MainTex("UNUSED", 2D) = "white" {}// to rid the lack of main tex property without breaking texture links this was added. please do not remove!


	}
		SubShader{
		Tags{ "Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "TransparentCutout" }
		LOD 200
		Blend SrcAlpha One
		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Color(0,0,0,0) }

		ZTest LEqual
		ColorMask RGBA

		CGPROGRAM
		#pragma surface surf Lambert keepalpha
		#pragma multi_compile _ MASKDISTORT_ON
		#pragma multi_compile _ UVSET2_ON
		#pragma multi_compile _ HORIZBIAS_ON

		sampler2D _Tex1;
		sampler2D _Tex1b;
		sampler2D _MaskTex;
		sampler2D _DisTex;
		float _Brightness;
		float _Tex1Bright;
		float _Tex1bBright;
		float _DisBias;
		float _DisBiasY;
		float _Alpha;
		float _Tex1bAmount;
		float4 _Pan1;
		float4 _Pan1b;
		float4 _DisPan;
		float4 _MaskPan;
		float4 _Color;
		float _Rot1;
		float _Rot2;
		float _RotMask;
		float _Rot1Pan;
		float _Rot2Pan;
		float _RotMaskPan;

		struct Input {
			float2 uv_Tex1;
			float2 uv_Tex1b;
			float2 uv_MaskTex;
			float2 uv_DisTex;
			float2 uv2_Tex1;
			float2 uv2_Tex1b;
			float2 uv2_MaskTex;
			float2 uv2_DisTex;
	};


	void surf(Input IN, inout SurfaceOutput o) {
		// get tex value first

		float s1 = sin((_Time.z * _Rot1Pan) + _Rot1);
		float c1 = cos((_Time.z * _Rot1Pan) + _Rot1);
		float2x2 rotationMatrix1 = float2x2(c1, -s1, s1, c1);
		float s2 = sin((_Time.z * _Rot2Pan) + _Rot2);
		float c2 = cos((_Time.z * _Rot2Pan) + _Rot2);
		float2x2 rotationMatrix2 = float2x2(c2, -s2, s2, c2);
		float s3 = sin((_Time.z * _RotMaskPan) + _RotMask);
		float c3 = cos((_Time.z * _RotMaskPan) + _RotMask);
		float2x2 rotationMatrix3 = float2x2(c3, -s3, s3, c3);

		fixed2 center = fixed2(0.5, 0.5);


		float4 distex = tex2D(_DisTex, IN.uv2_DisTex + float2(_Time.x * _DisPan.x * -2, _Time.x * (_DisPan.y * (-1))));////uv2
		float4 masktex = tex2D(_MaskTex, (mul(rotationMatrix3, IN.uv2_MaskTex - center) + center) + float2(_Time.x * _MaskPan.x * -2, _Time.x * (_MaskPan.y * (-1))));

		#ifdef MASKDISTORT_ON		

			// use r,g from first distort tex to distort tex1_uv in x,y//
			float dispuvX = distex.r*masktex.r * _DisBias * 0.1;
			float dispuv2X = distex.r * _DisBias * 0.1;
			#ifdef HORIZBIAS_ON
				float dispuvY = distex.g*masktex.g * _DisBiasY * 0.1;
				float dispuv2Y = distex.g * _DisBias * 0.1;
			#else
				float dispuvY = distex.g*masktex.g * _DisBias * 0.1;
				float dispuv2Y = distex.g * _DisBias * 0.1;
			#endif

			#ifdef UVSET2_ON
				float4 tex1 = tex2D(_Tex1, (mul(rotationMatrix1, IN.uv2_Tex1 - center) + center) + float2(dispuvX + _Time.x * _Pan1.x * -2, dispuvY + _Time.x*_Pan1.y * -1));
				float4 tex1b = tex2D(_Tex1b, (mul(rotationMatrix2, IN.uv2_Tex1b - center) + center) + float2(dispuv2X + _Time.x * _Pan1b.x * -2, dispuv2Y + _Time.x * _Pan1b.y * -1));
			#else
				float4 tex1 = tex2D(_Tex1, (mul(rotationMatrix1, IN.uv_Tex1 - center) + center) + float2(dispuvX + _Time.x * _Pan1.x * -2, dispuvY + _Time.x*_Pan1.y * -1));
				float4 tex1b = tex2D(_Tex1b, (mul(rotationMatrix2, IN.uv_Tex1b - center) + center) + float2(dispuv2X + _Time.x * _Pan1b.x * -2, dispuv2Y + _Time.x * _Pan1b.y * -1));
			#endif

			o.Alpha = tex1.a * _Alpha;

		#else

			// use r,g from first distort tex to distort tex1_uv in x,y//
			float dispuvX = distex.r * _DisBias * 0.1;
			#ifdef HORIZBIAS_ON
				float dispuvY = distex.g * _DisBiasY * 0.1;
			#else
				float dispuvY = distex.g * _DisBias * 0.1;
			#endif

			#ifdef UVSET2_ON
				float4 tex1 = tex2D(_Tex1, (mul(rotationMatrix1, IN.uv2_Tex1 - center) + center) + float2(dispuvX + _Time.x * _Pan1.x * -2, dispuvY + _Time.x*_Pan1.y * -1));
				float4 tex1b = tex2D(_Tex1b, (mul(rotationMatrix2, IN.uv2_Tex1b - center) + center) + float2(dispuvX + _Time.x * _Pan1b.x * -2, dispuvY + _Time.x * _Pan1b.y * -1));
			#else
				float4 tex1 = tex2D(_Tex1, (mul(rotationMatrix1, IN.uv_Tex1 - center) + center) + float2(dispuvX + _Time.x * _Pan1.x * -2, dispuvY + _Time.x*_Pan1.y * -1));
				float4 tex1b = tex2D(_Tex1b, (mul(rotationMatrix2, IN.uv_Tex1b - center) + center) + float2(dispuvX + _Time.x * _Pan1b.x * -2, dispuvY + _Time.x * _Pan1b.y * -1));
			#endif

			o.Alpha = tex1.a * _Alpha * masktex.a;

		#endif

		o.Emission = _Color.rgb * (((tex1.rgb + _Tex1Bright) * (1 - _Tex1bAmount)) + ((tex1b.rgb + _Tex1bBright)* tex1.rgb * _Tex1bAmount)) * _Brightness;

	}

	ENDCG
	}
		FallBack "Mobile/Particles/Additive"
	}