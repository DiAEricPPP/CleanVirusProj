Shader "Unlit/螺旋菌"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_WaveStrength("Wave Strength",Float) = 0.01
        _WaveFactor("Wave Factor",Float) = 50
        _TimeScale("Time Scale",Float) = 10

		_Color("Color",COLOR) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }

		 Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _WaveStrength;
			float _WaveFactor;
			float _TimeScale;
			float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //计算出fixed2(0.5,0.5)到uv每个点的单位向量方向
                fixed2 uvDir = normalize(i.uv-fixed2(0.5,0));
				//计算出fixed2(0.5,0.5)到uv每个点的距离
                fixed dis = distance(i.uv,fixed2(0.5,0));

                fixed2 uv = i.uv+_WaveStrength*uvDir*sin(_Time.y*_TimeScale+dis*_WaveFactor);

                fixed4 col = tex2D(_MainTex, uv) * _Color;
				fixed4 white = _Color;
             //   return white * col.a;
			 return col;
            }
            ENDCG
        }
    }
}
