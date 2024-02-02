Shader "Hidden/WeSpaceVisualizer" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
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
                float4 col = tex2D(_MainTex, i.uvlocal.xy);
                return col;
            }
            ENDCG
        }
    }
}
