 Shader "Custom/PointCloud"
{
	Properties{
		Size("Size", Float) = 5.0
		FadeIn("FadeIn", Range(0.0,1.0)) = 1.0
	}
		SubShader{
		   Pass {
			  CGPROGRAM
			  #pragma vertex vert
			  #pragma fragment frag

			  #include "UnityCG.cginc"

			  struct appdata
			  {
				 float4 vertex : POSITION;
				 float4 color: COLOR;
			  };

			  struct v2f
			  {
				 float4 vertex : SV_POSITION;
				 float4 color : COLOR;
				 float size : PSIZE;
			  };

			  float Size;
			  float FadeIn;

			  v2f vert(appdata v)
			  {
				 v2f o;
				 v.vertex.y = lerp(v.vertex.y-0.1,v.vertex.y,FadeIn);
				 o.vertex = UnityObjectToClipPos(v.vertex);
				 o.size = lerp(0.0,Size,FadeIn);
				 o.color = v.color;
				 return o;
			  }

			  fixed4 frag(v2f i) : SV_Target
			  {
				 return i.color;
			  }
			  
			  
			  
			  ENDCG
		   }
		   
		   
	}
	
}
