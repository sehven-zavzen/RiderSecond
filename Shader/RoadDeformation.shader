Shader "Custom/RoadDeformation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlattenDistance ("Flatten Distance", Float) = 30.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            float4 _PlayerWorldPos;
            float _FlattenDistance;

            Varyings vert (Attributes v)
            {
                Varyings o;
                // 1. Vertex'in dünyadaki gerçek (kıvrımlı) pozisyonu
                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz);
                
                // 2. Oyuncuya olan uzaklık (Player Z=0 hattında)
                float dist = abs(worldPos.z - _PlayerWorldPos.z);
                
                // 3. Düzleşme faktörü
                float flattenFactor = 1.0 - saturate(dist / _FlattenDistance);
                
                // --- MANTIK: DÜNYA KIVRIMINI SİL, YEREL FORMU KORU ---
                // v.positionOS.x: Yolun mesh üzerindeki orijinal genişliği (Değişmemeli).
                // worldPos.x: Yolun dünyadaki kıvrılmış hali.
                
                // Yaklaştıkça dünyadaki kıvrımı (worldPos.x) tamamen iptal edip,
                // mesh'in orijinal halini (v.positionOS.x) dünya merkezine (0) oturtuyoruz.
                float targetX = v.positionOS.x; 
                float targetY = 0.0; 

                worldPos.x = lerp(worldPos.x, targetX, flattenFactor);
                worldPos.y = lerp(worldPos.y, targetY, flattenFactor);
                
                o.positionCS = TransformWorldToHClip(worldPos);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target { return tex2D(_MainTex, i.uv); }
            ENDHLSL
        }
    }
}