Shader "Unlit/ShaderST"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)

        _Velocity("Velocity", Vector) = (0,0,0,0)

        _C("C", Float) = 1

        _PrevPosCount("PrevPosCount", Float) = 64
        _PrevPosCurrentIndex("PrevPosCurrentIndex", Float) = 0
        _PrevPosCurrentTime("PrevPosCurrentTime", Float) = 0
    }
    SubShader
    {

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile ADVANCED_TIMEBACK_OFF ADVANCED_TIMEBACK_ON
            #pragma multi_compile LSD_OFF LSD_ON

            #pragma multi_compile COLOR_BY_TIME_ON COLOR_BY_TIME_OFF

            #include "UnityCG.cginc"
            #include "ShaderFunc.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

                float timeBack : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _C;
            float4 _Velocity;

            float4 _Color;

            struct PrevPosData
            {
                float4 positionST : POSITION;
            };
            int _PrevPosCount;
            int _PrevPosCurrentIndex;
            float _PrevPosCurrentTime;

            StructuredBuffer<PrevPosData> _PrevPosBuffer;

            v2f vert (appdata IN)
            {
                v2f OUT;

                //view pos
                float4 vertex = mul(UNITY_MATRIX_M,IN.vertex);
                float4 vel = _Velocity;

                float3 worldModelPos = UNITY_MATRIX_M[3].xyz;
                float3 vertexWorldOffset = vertex.xyz - worldModelPos;

                float4 timeBackPos = vertex;

                #ifdef ADVANCED_TIMEBACK_OFF
                    //binary search the buffer

                    int minBound = 0;
                    int maxBound = _PrevPosCount;
                    for (int i = 0; i < log2(_PrevPosCount); i++)
                    {
                        int index = minBound + (maxBound-minBound)/2;
                        int bufferIndex = _PrevPosCurrentIndex-index;
                        if (bufferIndex < 0) bufferIndex += _PrevPosCount;
                        PrevPosData data = _PrevPosBuffer[bufferIndex];

                        float3 positionST = (data.positionST.xyz+vertexWorldOffset)/_C;
                        float realtiveTime = data.positionST.w - _PrevPosCurrentTime;

                        bool outCone = dot(positionST,positionST) > realtiveTime*realtiveTime;
                        if (outCone)
                        {
                            minBound = index;
                        }
                        else
                        {
                            maxBound = index;
                        }
                    }

                    //use data at index from binary search
                    int timeBackBufferIndex = _PrevPosCurrentIndex - minBound;

                    if (timeBackBufferIndex < 0) 
                    {
                        timeBackBufferIndex += _PrevPosCount;
                    }
                    PrevPosData timeBackData = _PrevPosBuffer[timeBackBufferIndex];

                    OUT.timeBack = timeBackData.positionST.w-_PrevPosCurrentTime;
                    timeBackPos = timeBackData.positionST + float4(vertexWorldOffset.xyz,0);
                    timeBackPos.w = 1;

                #else
                    //based on how far the object is, roll back time a bit, this is cause the light takes a bit to travel

                    float4 pos_C = vertex/_C; //just the position adjusted for the speed of light
                    float4 vel_C = vel/_C; //just the position adjusted for the speed of light

                    float posVel_C_dot = dot(pos_C,vel_C);
                    float vel_C_SqrMag = dot(vel_C,vel_C);
                    float pos_C_SqrMag = dot(pos_C,pos_C);

                    float timeStepBack = ( -(posVel_C_dot) + sqrt( (posVel_C_dot*posVel_C_dot) - (vel_C_SqrMag - 1)*(pos_C_SqrMag) )) / (vel_C_SqrMag - 1);

                    OUT.timeBack = timeStepBack;

                    timeBackPos = vertex + vel*timeStepBack;
                #endif

                //go back in time
                #ifdef LSD_ON
                    vertex = timeBackPos;
                #endif

                vertex = mul(UNITY_MATRIX_V,vertex);
                vertex = mul(UNITY_MATRIX_P,vertex);

                OUT.vertex = vertex;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return OUT;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                // sample the texture
                fixed4 texCol = tex2D(_MainTex, IN.uv);

                fixed4 realCol = texCol * _Color;

                
                #ifdef COLOR_BY_TIME_ON
                    float3 timeColor = HSVToRGB(float3(IN.timeBack*0.4f,1,1));
                    realCol = fixed4(timeColor.x,timeColor.y,timeColor.z,1);
                #endif
                

                return realCol;
            }
            ENDCG
        }
    }
}
