Shader "Custom/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _KernelSize("Kernel Size (N)", Int) = 3        
    }
    SubShader
    {
        // Alpha transparency
		Tags{
		     "RenderType"="Transparent"
		     "Queue"="Transparent"
        }

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite off

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
           
            sampler2D _MainTex;
            float2 _MainTex_TexelSize;
            int _KernelSize;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 sum = fixed4(0.0, 0.0, 0.0, 0.0);
                
                for (int x = 0; x < _KernelSize; ++x)
                {
                    for (int y = 0; y < _KernelSize; ++y)
                    {
                        const fixed2 offset = fixed2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
                        sum += tex2D(_MainTex, i.uv + offset);
                    }
                }

                sum /= (_KernelSize * _KernelSize);
                return sum * i.color;
            }
            ENDCG
        }
    }
}
