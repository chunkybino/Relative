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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _C;
            float4 _Velocity;

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

                //view pos
                o.vertex = mul(UNITY_MATRIX_M,v.vertex);
                o.vertex = mul(UNITY_MATRIX_V,o.vertex);

                #ifdef LSD_ON
                    float4 vel = mul(UNITY_MATRIX_V,_Velocity);

                    //based on how far the object is, roll back time a bit, this is cause the light takes a bit to travel

                    float4 pos_C = o.vertex/_C; //just the position adjusted for the speed of light
                    float4 vel_C = vel/_C; //just the position adjusted for the speed of light

                    float posVel_C_dot = dot(pos_C,vel_C);
                    float vel_C_SqrMag = dot(vel_C,vel_C);
                    float pos_C_SqrMag = dot(pos_C,pos_C);

                    float timeStepBack = ( -(posVel_C_dot) + sqrt( (posVel_C_dot*posVel_C_dot) - (vel_C_SqrMag - 1)*(pos_C_SqrMag) )) / (vel_C_SqrMag - 1);

                    o.vertex += vel*timeStepBack;
                #endif

                o.vertex = mul(UNITY_MATRIX_P,o.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                col *= _Color;

                return col;
            }
            ENDCG
        }
    }
}
