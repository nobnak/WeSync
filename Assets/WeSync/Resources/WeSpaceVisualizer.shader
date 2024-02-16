Shader "Hidden/WeSpaceVisualizer" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Wireframe_Gain ("Wireframe width", Float) = 1.5
        _PositionScale ("Position Scale", Float) = 0
    }
    SubShader {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Packages/jp.nobnak.wesync/Resources/WeSync_Common.cginc"
            #include "Packages/jp.nobnak.wireframe/ShaderLIbrary/Wireframe.cginc"
            #include "Packages/jp.nobnak.ascii_shader/ShaderLibrary/FontTexture.hlslinc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 uv : TEXCOORD0;
	            float4 uvlocal : TEXCOORD1;
	            float4 pos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            int _PositionScale;

            v2f vert (appdata v) {
	            float4 uvlocal = float4(v.uv, uv2npos_local(v.uv));
	            float4 uvglobal = float4(local2global_uv(uvlocal.xy), local2global_npos(uvlocal.zw));
	            float4 pos = float4(guv2wpos(v.uv), uv2wpos(v.uv));

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
	            o.uv = uvglobal;
	            o.uvlocal = uvlocal;
	            o.pos = pos;
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                float4 cmain = tex2D(_MainTex, i.uvlocal.xy);
                
                float2 pos = i.pos.zw;
                float2 pos_scaled = pos * pow(2, _PositionScale);
                float4 p_repeat = float4(frac(pos_scaled), 0, 0);
                p_repeat.zw = 1 - p_repeat.xy;

                int2 code = floor(pos_scaled);
                int4 ascii = int4(
                    FontTexture_IndexOf(code.y / 10)[1],
                    FontTexture_IndexOf(code.y % 10)[0],
                    FontTexture_IndexOf(code.x / 10)[1],
                    FontTexture_IndexOf(code.x % 10)[0]
                );
                float4 f = FontTexture_GetText4(p_repeat * 3, ascii);

                float w = wireframe(p_repeat);
                w += f.x;
                return float4(lerp(cmain.xyz, 1 - cmain.xyz, saturate(w)), cmain.w);
            }
            ENDCG
        }
    }
}
