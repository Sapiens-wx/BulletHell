Shader "Unlit/Background"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        spd ("spd",Float)=0
        noiseScale("noise scale", Float)=0
        noiseAlpha("noise alpha", Float)=0
        brightness("brightness", Float)=0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float spd,noiseScale,noiseAlpha;
            float brightness;

            float rand(float seed){
                return frac(sin(seed)*43758.5453123);
            }
            float2 rand2(float2 st){
                st = float2( dot(st,float2(127.1,311.7)),
                        dot(st,float2(269.5,183.3)) );
                return frac(sin(st)*38673.918236);
            }
            float noise(float seed){
                float i = floor(seed);  // integer
                float f = frac(seed);  // fraction
                return lerp(rand(i), rand(i + 1), smoothstep(0.,1.,f));
            }
            float2 noise2(float seed){
                return float2(noise(seed),noise(seed+1432.432));
            }
            float perlin(float2 st) {
                float2 i = floor(st);
                float2 f = frac(st);

                float2 u = f*f*(3.0-2.0*f);

                return lerp( lerp( dot( rand2(i + float2(0.0,0.0) ), f - float2(0.0,0.0) ),
                                dot( rand2(i + float2(1.0,0.0) ), f - float2(1.0,0.0) ), u.x),
                            lerp( dot( rand2(i + float2(0.0,1.0) ), f - float2(0.0,1.0) ),
                                dot( rand2(i + float2(1.0,1.0) ), f - float2(1.0,1.0) ), u.x), u.y);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv=i.uv+noise2(_Time.y*spd);
                fixed4 col = tex2D(_MainTex, uv)*(perlin(uv*noiseScale)+noiseAlpha);
                uv=i.uv+noise2(_Time.y*spd*1.243);
                col=lerp(col,tex2D(_MainTex, uv),.3)*(perlin(uv*noiseScale)+noiseAlpha);
                uv=i.uv+noise2(_Time.y*spd*.56473);
                col=lerp(col,tex2D(_MainTex, uv),.3)*(perlin(uv*noiseScale)+noiseAlpha);
                col*=brightness;
                return col;
            }
            ENDCG
        }
    }
}
