Shader "Custom/RoadDeformation_UV2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _FlattenDistance ("Flatten Distance (Forward)", Float) = 40.0
        _FlattenLookAhead ("Look Ahead (Z)", Float) = 15.0

        _FlattenYToZero ("Flatten Y To Zero (0/1)", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float2 uv2        : TEXCOORD1; // UV2.x = düz hedef local X
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _PlayerWorldPos;
            float  _FlattenDistance;
            float  _FlattenLookAhead;
            float  _FlattenYToZero;

            Varyings vert (Attributes v)
            {
                Varyings o;

                // Player'ı object space'e çevir (space mismatch olmasın)
                float3 playerOS = TransformWorldToObject(_PlayerWorldPos.xyz);

                // Düzleşme referans noktasını player'ın önüne al
                float refZ = playerOS.z + _FlattenLookAhead;

                // Sadece ileri tarafı düzleştir (arkayı hiç etkileme)
                float dz = v.positionOS.z - refZ;     // vertex ref'ten ne kadar ileride?
                float distForward = max(dz, 0.0);     // arkaysa 0

                float denom = max(_FlattenDistance, 0.0001);
                float flattenFactor = 1.0 - saturate(distForward / denom);  // ref'e yakın=1, uzakta=0

                // UV2.x -> yolun düz (orijinal form) X hedefi
                float straightX = v.uv2.x;

                float3 posOS = v.positionOS.xyz;

                // X: kıvrımlı x -> düz x
                posOS.x = lerp(posOS.x, straightX, flattenFactor);

                // Y: istersen 0'a çek (0/1 ile kontrol)
                // _FlattenYToZero = 1 ise hedefY=0, 0 ise hedefY=posOS.y (yani dokunma)
                float targetY = lerp(posOS.y, 0.0, saturate(_FlattenYToZero));
                posOS.y = lerp(posOS.y, targetY, flattenFactor);

                float3 worldPos = TransformObjectToWorld(posOS);
                o.positionCS = TransformWorldToHClip(worldPos);
                o.uv = v.uv;

                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
            }
            ENDHLSL
        }
    }
}
