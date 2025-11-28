Shader "Unlit/ShaderST"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)

        _BasePos("BasePos", Vector) = (0,0,0,0)
        _BaseVel("BaseVel", Vector) = (0,0,0,0)
        _RealVel("RealVel", Vector) = (0,0,0,0)

        _BaseTime("BaseTime", Float) = 0
        _ProperTime("ProperTime", Float) = 0

        _C("C", Float) = 1

        _BaseLengthContractionVector("BaseLengthContractionVector", Vector) = (0,0,0,1)
        _RealLengthContractionVector("RealLengthContractionVector", Vector) = (0,0,0,1)

        _FramePos("FramePos", Vector) = (0,0,0,0)
        _FrameVel("FrameVel", Vector) = (0,0,0,0)

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

            #pragma multi_compile COLOR_BY_TIME_OFF COLOR_BY_TIME_ON

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

            float4 _BaseLengthContractionVector;
            float4 _RealLengthContractionVector;

            float3 _BasePos;
            float3 _BaseVel;
            float3 _RealVel;

            float _BaseTime;
            float _ProperTime;

            float3 _FramePos;
            float3 _FrameVel;

            float4 _Color;

            struct PrevPosData
            {
                float4 positionST : POSITION; //w coord is time
                float4 velocity : TEXCOORD0; //w coord of velocity is length contraction factor (1/gamma)
            };
            int _PrevPosCount;
            int _PrevPosCurrentIndex;
            float _PrevPosCurrentTime;

            StructuredBuffer<PrevPosData> _PrevPosBuffer;

            v2f vert (appdata IN)
            {
                v2f OUT;

                float4x4 frameVelMatrix = LorentzBoost(_FrameVel, _C);
                float4x4 frameVelMatrixInverse = LorentzBoost(-_FrameVel, _C);

                //view pos
                float4 vertex = mul(UNITY_MATRIX_M,IN.vertex);

                float3 worldModelPos = float3(UNITY_MATRIX_M[0].w,UNITY_MATRIX_M[1].w,UNITY_MATRIX_M[2].w);
                float3 vertexWorldOffset = vertex.xyz - worldModelPos;

                //vertex = float4(worldModelPos + vertexWorldOffset, 1);
                vertex = float4(worldModelPos + ScaleInDirection(vertexWorldOffset, _RealLengthContractionVector), 1);

                float4 timeBackPos = vertex;

                #ifdef ADVANCED_TIMEBACK_ON

                    //binary search the buffer

                    /*
                    int minBound = 0;
                    int maxBound = _PrevPosCount;
 
                    for (int i = 0; i < log2(_PrevPosCount); i++)
                    {
                        int index = minBound + (maxBound-minBound)/2;
                        int bufferIndex = _PrevPosCurrentIndex-index;
                        if (bufferIndex < 0) bufferIndex += _PrevPosCount;
                        PrevPosData data = _PrevPosBuffer[bufferIndex];

                        float3 positionXYZ = data.positionST.xyz + ApplyLengthContraction(vertexWorldOffset, data.velocity.xyz, _C) - _FramePos;

                        float realtiveTime = data.positionST.w - _PrevPosCurrentTime;

                        float4 positionST = float4(positionXYZ/_C, realtiveTime);

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
                    //PrevPosData timeBackData = _PrevPosBuffer[_PrevPosCurrentIndex];
                    PrevPosData timeBackData = _PrevPosBuffer[timeBackBufferIndex];
                    */
 
                    float4 posDif = _BasePos-float4(_FramePos,0);
                    posDif.w = -sqrt(dot(posDif.xyz,posDif.xyz)) / _C;

                    //OUT.timeBack = timeBackData.positionST.w-_PrevPosCurrentTime;
                    //timeBackPos = float4(timeBackData.positionST.xyz + ApplyLengthContraction(vertexWorldOffset, timeBackData.velocity.xyz, _C), -OUT.timeBack);

                    //timeBackPos = mul(frameVelMatrix, timeBackData.positionST.xyz); 
                    timeBackPos.w = 0;
                    timeBackPos = mul(frameVelMatrix+vertexWorldOffset, posDif); 
                    //timeBackPos = mul(frameVelMatrix, vertex); 

                    timeBackPos.w = 1;

                #else
                    //based on how far the object is, roll back time a bit, this is cause the light takes a bit to travel

                    float3 pos_C = vertex/_C; //just the position adjusted for the speed of light
                    float3 vel_C = _RealVel/_C; //just the velocity adjusted for the speed of light

                    float posVel_C_dot = dot(pos_C,vel_C);
                    float vel_C_SqrMag = dot(vel_C,vel_C);
                    float pos_C_SqrMag = dot(pos_C,pos_C);

                    float realTimeStepBack = ( -(posVel_C_dot) + sqrt( (posVel_C_dot*posVel_C_dot) - (vel_C_SqrMag - 1)*(pos_C_SqrMag) )) / (vel_C_SqrMag - 1);

                    timeBackPos = float4(vertex + _RealVel*realTimeStepBack, realTimeStepBack);

                    timeBackPos.w = 1;

                    #ifdef LSD_ON
                        vertex = timeBackPos;
                        OUT.timeBack = realTimeStepBack / Gamma(_RealVel, _C);
                        OUT.timeBack += _ProperTime + dot(_FrameVel, vertexWorldOffset)/(_C*_C);
                    #else
                        OUT.timeBack = _ProperTime + dot(_FrameVel, vertexWorldOffset)/(_C*_C);
                    #endif

                    
                #endif

                //go back in time
                #ifdef LSD_ON
                    vertex = timeBackPos;
                #endif

                vertex = mul(UNITY_MATRIX_V,vertex);
                vertex = mul(UNITY_MATRIX_P,vertex);

                OUT.vertex = vertex;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                // sample the texture
                fixed4 texCol = tex2D(_MainTex, IN.uv);

                fixed4 realCol = texCol * _Color;

                
                #ifdef COLOR_BY_TIME_ON
                    float3 timeColor = HSVToRGB(float3(IN.timeBack*0.1f,1,1));
                    realCol = fixed4(timeColor.x,timeColor.y,timeColor.z,1);
                #endif
                

                return realCol;
            }
            ENDCG
        }
    }
}
