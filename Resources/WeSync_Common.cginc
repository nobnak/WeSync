#ifndef __WESYNC__COMMON__CGINC__
#define __WESYNC__COMMON__CGINC__



#include "UnityCG.cginc"

float4 _We_Time;
float4x4 _We_Local2Global;
float4x4 _We_Uv2Npos;
float4x4 _We_Uv2Pos;


float2 trs(float4 m, float2 x) { return m.xy * x + m.zw; }

float2 uv2pos_local(float2 uv) { return trs(_We_Uv2Pos[0], uv); }
float2 uv2pos_global(float2 uv) { return trs(_We_Uv2Pos[1], uv); }
float2 uv2pos_local_inv(float2 x) { return trs(_We_Uv2Pos[2], x); }
float2 uv2pos_global_inv(float2 x) { return trs(_We_Uv2Pos[3], x); }

float2 uv2npos_local(float2 uv) { return trs(_We_Uv2Npos[0], uv); }
float2 uv2npos_global(float2 uv) { return trs(_We_Uv2Npos[1], uv); }
float2 uv2npos_local_inv(float2 npos) { return trs(_We_Uv2Npos[2], npos); }
float2 uv2npos_global_inv(float2 npos) { return trs(_We_Uv2Npos[3], npos); }

float2 local2global_uv(float2 uv) { return trs(_We_Local2Global[0], uv); }
float2 local2global_npos(float2 npos) { return trs(_We_Local2Global[1], npos); }
float2 local2global_uv_inv(float2 uv) { return trs(_We_Local2Global[2], uv); }



float globalTime() { return _We_Time.y; }



#endif
