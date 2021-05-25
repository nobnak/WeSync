#ifndef __WESYNC__COMMON__CGINC__
#define __WESYNC__COMMON__CGINC__



#include "UnityCG.cginc"

struct appdata {
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f {
    float4 uv : TEXCOORD0;
	float4 uvlocal : TEXCOORD1;
    float4 vertex : SV_POSITION;
};

float4 _We_Time;
float4x4 _We_Local2Global;
float4x4 _We_Uv2Npos;

float2 uv2npos_local(float2 uv) { 
    float4 m = _We_Uv2Npos[0];
    return m.xy * uv + m.zw;
}
float2 uv2npos_global(float2 uv) {
    float4 m = _We_Uv2Npos[1];
    return m.xy * uv + m.zw;
}
float2 uv2npos_local_inv(float2 npos) {
    float4 m = _We_Uv2Npos[2];
    return m.xy * npos + m.zw;
}
float2 uv2npos_global_inv(float2 npos) {
    float4 m = _We_Uv2Npos[3];
    return m.xy * npos + m.zw;
}

float2 local2global_uv(float2 uv) {
    float4 m = _We_Local2Global[0];
    return m.xy * uv + m.zw;
}
float2 local2global_npos(float2 npos) {
    float4 m = _We_Local2Global[1];
    return m.xy * npos + m.zw;
}
float2 local2global_uv_inv(float2 uv) {
    float4 m = _We_Local2Global[2];
    return m.xy * uv + m.zw;
}



#endif
