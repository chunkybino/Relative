#ifndef SHADERFUNC_HLSL
#define SHADERFUNC_HLSL

float3 HSVToRGB(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * lerp(K.xxx, saturate(p - K.x), c.y);
}

#endif