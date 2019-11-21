Shader "UCLA Game Lab/Wireframe/Single-Sided Custom" 
{
	Properties 
	{
		_Color ("Line Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_Thickness ("Thickness", Float) = 1
		_BackgroundColor ("Background Color", Color) = (0,0,0,1)
		_BackgroundTex("Background Texture", 2D) = "white" {}
		_WaveFactor ("wave factor", Float) = 0.5
	}

	SubShader 
	{
		Pass
		{
			Tags { "RenderType"="Opaque" "Queue"="Geometry" }

			Blend SrcAlpha OneMinusSrcAlpha 
			LOD 200
			
			CGPROGRAM
				#pragma target 5.0
				#include "UnityCG.cginc"
				#include "UCLA GameLab Wireframe Functions.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma geometry geom

				float4 _BackgroundColor;
				float _WaveFactor;
				sampler2D _BackgroundTex;

				// Vertex Shader
				UCLAGL_v2g vert(appdata_base v)
				{
					return UCLAGL_vert(v);
				}
				
				// Geometry Shader
				[maxvertexcount(3)]
				void geom(triangle UCLAGL_v2g p[3], inout TriangleStream<UCLAGL_g2f> triStream)
				{
					UCLAGL_geom( p, triStream);
				}
				
				// Fragment Shader
				float4 frag(UCLAGL_g2f input) : COLOR
				{	
					float4 col = UCLAGL_frag(input);
					float wave = 1.0f-_WaveFactor + (1+cos(-input.uv.x + -input.uv.y + _Time.y))/2* _WaveFactor;
					float4 backgroundColor = _BackgroundColor * tex2D(_BackgroundTex, input.uv);

					col.r = col.a * col.r + (1-col.a)* backgroundColor.r ;
					col.g = col.a * col.g + (1 - col.a) * backgroundColor.g ;
					col.b = col.a * col.b + (1 - col.a) * backgroundColor.b ;
					col = col * wave;
					col.a = 1;
					
					return col;
				}
			
			ENDCG
		}
	} 
}
