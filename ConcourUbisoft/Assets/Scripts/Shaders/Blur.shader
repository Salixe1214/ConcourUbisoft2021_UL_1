Shader "Custom/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _KernelSize("Kernel Size (N)", Int) = 3
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            float2 _MainTex_TexelSize;
            int _KernelSize;
            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 sum = fixed3(0.0, 0.0, 0.0);

                const int upper = ((_KernelSize - 1) / 2);
                const int lower = -upper;

                for (int x = lower; x <= upper; ++x)
                {
                    for (int y = lower; y <= upper; ++y)
                    {
                        const fixed2 offset = fixed2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
                        sum += tex2D(_MainTex, i.uv + offset);
                    }
                }

                sum /= (_KernelSize * _KernelSize);
                return fixed4(sum + i.color, 1.0);
            }
            ENDCG
        }
    }
}
