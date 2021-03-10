Shader "Unlit/breath"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_WaveStrength("Wave Strength",Float) = 0.01
        _WaveFactor("Wave Factor",Float) = 50
        _TimeScale("Time Scale",Float) = 10

		_Color("Color",COLOR) = (1,1,1,1)

		_BurnMap("BurnMap",2D) = "white"{}
		_BurnScale("BurnScale",float) = 0.1

		_BurnColor1("BurnColor1",COLOR) = (1,1,1,1)
		_BurnColor2("BurnColor2",COLOR) = (1,1,1,1)
		_LineWidth("LineWidth",float) = 0.5
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
			//	float4 uvB :TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 uvB:TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _WaveStrength;
			float _WaveFactor;
			float _TimeScale;
			float4 _Color;
			sampler2D _BurnMap;
			float4 _BurnMap_ST;
			float _BurnScale;

			float4 _BurnColor1;
			float4 _BurnColor2;
			float _LineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.uvB = TRANSFORM_TEX(v.uv,_BurnMap);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //计算出fixed2(0.5,0.5)到uv每个点的单位向量方向
                fixed2 uvDir = normalize(i.uv-fixed2(0.5,0.5));
				//计算出fixed2(0.5,0.5)到uv每个点的距离
                fixed dis = distance(i.uv,fixed2(0.5,0.5));

                fixed2 uv = i.uv+_WaveStrength*uvDir*sin(_Time.y*_TimeScale+dis*_WaveFactor);
				fixed4 burn =  tex2D(_BurnMap,i.uvB) * _Color;
                fixed4 col = tex2D(_MainTex, uv);

				clip(burn.r - _BurnScale*0.1);

				float t = 1 - smoothstep(0.0,_LineWidth,burn.r-_BurnScale*0.1);
				float3 fixedColor = lerp(_BurnColor1,_BurnColor2,t);
				fixedColor = pow(fixedColor,5);


				col += burn/10 ;
				fixed4 white = _Color;

				fixed3 color = lerp(col, fixedColor, t * step(0.0001, _BurnScale));
             //   return white * col.a;
			 return fixed4( color,col.a);
            }
            ENDCG
        }
    }
}
