#ifndef __WESYNC__COMMON__CGINC__
#define __WESYNC__COMMON__CGINC__

#include "UnityCG.cginc"

float4 _We_Time;
float4x4 _We_Local2Global;
float4x4 _We_Uv2Npos;
float4x4 _We_Uv2Pos;

#define UV2WPOS _We_Uv2Pos[0]
#define GUV2WPOS _We_Uv2Pos[1]
#define UV2WPOS_INV _We_Uv2Pos[2]
#define GUV2WPOS_INV _We_Uv2Pos[3]

#define UV2NPOS_LOCAL _We_Uv2Npos[0]
#define UV2NPOS_GLOBAL _We_Uv2Npos[1]
#define UV2NPOS_LOCAL_INV _We_Uv2Npos[2]
#define UV2NPOS_GLOBAL_INV _We_Uv2Npos[3]

#define LOCAL2GLOBAL_UV _We_Local2Global[0]
#define LOCAL2GLOBAL_NPOS _We_Local2Global[1]
#define LOCAL2GLOBAL_UV_INV _We_Local2Global[2]


float2 trs(float4 m, float3 x) { return m.xy * x.xy + m.zw * x.z; }

float spacialUnits_local() { return _We_Uv2Pos[0][1]; }
float spacialUnits_global() { return _We_Uv2Pos[1][1]; }

float2 uv2wpos(float2 uv) {
    return trs(UV2WPOS, float3(uv, 1));
}
float2 guv2wpos(float2 uv) {
    return trs(GUV2WPOS, float3(uv, 1));
}
float2 uv2wpos_inv(float2 x) {
    return trs(UV2WPOS_INV, float3(x, 1));
}
float2 guv2wpos_inv(float2 x) {
    return trs(GUV2WPOS_INV, float3(x, 1));
}

float2 uv2npos_local(float2 uv) {
    return trs(UV2NPOS_LOCAL, float3(uv, 1));
}
float2 uv2npos_global(float2 uv) {
    return trs(UV2NPOS_GLOBAL, float3(uv, 1));
}
float2 uv2npos_local_inv(float2 npos) {
    return trs(UV2NPOS_LOCAL_INV, float3(npos, 1));
}
float2 uv2npos_global_inv(float2 npos) {
    return trs(UV2NPOS_GLOBAL_INV, float3(npos, 1));
}

float2 local2global_uv(float2 uv) {
    return trs(LOCAL2GLOBAL_UV, float3(uv, 1));
}
float2 local2global_npos(float2 npos) {
    return trs(LOCAL2GLOBAL_NPOS, float3(npos, 1));
}
float2 local2global_uv_inv(float2 uv) {
    return trs(LOCAL2GLOBAL_UV_INV, float3(uv, 1));
}



float globalTime() { return _We_Time.y; }



#endif
