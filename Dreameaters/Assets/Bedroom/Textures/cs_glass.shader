

Shader "Glass Shader" {
        Properties {
                _Color ("Main Color", Color) = (1,1,1,1)
                _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
                _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
                _Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
                _ReflectStrength( "Reflection Strength" , Range(0,1)) = 0.5
                _Fresnel( "Fresnel " , Range(0,5)) = 1.0
        }
       
     
       
SubShader {
            
Tags { "Queue"="Transparent"  "RenderType" = "Transparent" }    


CGPROGRAM
#pragma surface surf Lambert alpha

struct Input  {

        float2 uv_MainTex;
        float3 viewDir;
                float3 worldRefl;
};

uniform sampler2D _MainTex;
uniform float4 _Color;
uniform samplerCUBE _Cube;
uniform float4 _ReflectColor;
uniform float _ReflectStrength;
uniform float _Fresnel;

void surf (Input IN, inout SurfaceOutput o) 
{
                
        float4 texcol = tex2D(_MainTex, IN.uv_MainTex);
        texcol.rgb *=_Color.rgb;
        texcol.a = texcol.a * _Color.a;
       
        float4 reflectCol = texCUBE( _Cube , IN.worldRefl );
                
        reflectCol *= _ReflectColor;
        reflectCol *= _ReflectStrength;
        
        float3 view = normalize( IN.viewDir );
        float fresnel = pow(1.0 - saturate ( dot ( view, o.Normal )), _Fresnel);
        
        texcol.rgb += reflectCol.rgb * fresnel ;
        
                o.Albedo = texcol.rgb;
                o.Alpha = texcol.a +  fresnel ;
}
ENDCG

 }
        
        FallBack "Transparent/Diffuse"
}