#ifndef SHADERFUNC_HLSL
#define SHADERFUNC_HLSL

float3 HSVToRGB(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.w);
    return c.z * lerp(K.xxx, saturate(p - K.x), c.y);
}

float3 ScaleInDirection(float3 vec, float4 scale) //scale xyz is scale direction, scale w is magnitude
{
    float3 inVector = scale*dot(vec,scale);
    float3 outVector = vec - inVector;

    return scale.w*inVector + outVector;
}

float Gamma(float3 vel, float C)
{
    return 1/sqrt(1-dot(vel,vel)/(C*C));
}
float GammaInverse(float3 vel, float C)
{
    return sqrt(1-dot(vel,vel)/(C*C));
}

float3 ApplyLengthContraction(float3 vec, float3 velocity, float C)
{
    if (dot(velocity,velocity) == 0) return vec;
    float4 lengthContractionVec = float4(normalize(velocity), GammaInverse(velocity, C));
    return ScaleInDirection(vec, lengthContractionVec);
}

float4x4 LorentzBoost(float3 vel, float C)
{
    vel = vel/C;
    float gamma = Gamma(vel, 1); //speed of light is 1 here because we already normalized the vel to C in the previous line

    float gamSqrWeird = gamma*gamma/(1+gamma);

    float4x4 mat = float4x4(
        1+gamSqrWeird*(vel.x*vel.x), gamSqrWeird*(vel.x*vel.y), gamSqrWeird*(vel.x*vel.z), -gamma*vel.x, 
        gamSqrWeird*(vel.x*vel.y),   1+gamSqrWeird*(vel.y*vel.y), gamSqrWeird*(vel.y*vel.z), -gamma*vel.y, 
        gamSqrWeird*(vel.x*vel.z),   gamSqrWeird*(vel.y*vel.z),   1+gamSqrWeird*(vel.z*vel.z), -gamma*vel.z,
        -gamma*vel.x, -gamma*vel.y, -gamma*vel.z, gamma
    );

    return mat;
}

#endif