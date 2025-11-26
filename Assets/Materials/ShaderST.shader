Shader "Unlit/ShaderST"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)

        _Velocity("Velocity", Vector) = (0,0,0,0)

        _C("C", Float) = 1
    }
    SubShader
    {

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            float4 _Velocity;

            float4 _Color;

            v2f vert (appdata IN)
            {
                v2f OUT;

                //view pos
                OUT.vertex = mul(UNITY_MATRIX_M,IN.vertex);
                OUT.vertex = mul(UNITY_MATRIX_V,OUT.vertex);

                float4 vel = mul(UNITY_MATRIX_V,_Velocity);

                //based on how far the object is, roll back time a bit, this is cause the light takes a bit to travel

                float4 pos_C = OUT.vertex/_C; //just the position adjusted for the speed of light
                float4 vel_C = vel/_C; //just the position adjusted for the speed of light

                float posVel_C_dot = dot(pos_C,vel_C);
                float vel_C_SqrMag = dot(vel_C,vel_C);
                float pos_C_SqrMag = dot(pos_C,pos_C);

                float timeStepBack = ( -(posVel_C_dot) + sqrt( (posVel_C_dot*posVel_C_dot) - (vel_C_SqrMag - 1)*(pos_C_SqrMag) )) / (vel_C_SqrMag - 1);

                OUT.timeBack = timeStepBack;

                //go back in time
                #ifdef LSD_ON
                    OUT.vertex += vel*timeStepBack;
                #endif

                OUT.vertex = mul(UNITY_MATRIX_P,OUT.vertex);

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
                    float3 timeColor = HSVToRGB(float3(IN.timeBack/3,1,1));
                    realCol = fixed4(timeColor.x,timeColor.y,timeColor.z,1);
                #endif

                return realCol;
            }
            ENDCG
        }
    }
}
