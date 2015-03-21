// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32713|emission-2-OUT,clip-4-OUT;n:type:ShaderForge.SFN_Multiply,id:2,x:33076,y:32701|A-3-RGB,B-4-OUT,C-5-OUT;n:type:ShaderForge.SFN_Color,id:3,x:33404,y:32560,ptlb:Glow,ptin:_Glow,glob:False,c1:0,c2:0.5,c3:1,c4:1;n:type:ShaderForge.SFN_OneMinus,id:4,x:33319,y:32752|IN-6-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5,x:33247,y:32948,ptlb:Bloom,ptin:_Bloom,glob:False,v1:1;n:type:ShaderForge.SFN_Clamp01,id:6,x:33482,y:32856|IN-7-OUT;n:type:ShaderForge.SFN_Add,id:7,x:33682,y:32783|A-10-R,B-8-OUT;n:type:ShaderForge.SFN_RemapRange,id:8,x:33920,y:32887,frmn:0,frmx:1,tomn:-1,tomx:1|IN-9-OUT;n:type:ShaderForge.SFN_Slider,id:9,x:34107,y:32954,ptlb:Dissolve,ptin:_Dissolve,min:0,cur:0.5470086,max:1;n:type:ShaderForge.SFN_Tex2d,id:10,x:34157,y:32752,ptlb:Dissolve Noise,ptin:_DissolveNoise,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;proporder:3-5-9-10;pass:END;sub:END;*/

Shader "Shader Forge/DissolveShader" {
    Properties {
        _Glow ("Glow", Color) = (0,0.5,1,1)
        _Bloom ("Bloom", Float ) = 1
        _Dissolve ("Dissolve", Range(0, 1)) = 0.5470086
        _DissolveNoise ("Dissolve Noise", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _Glow;
            uniform float _Bloom;
            uniform float _Dissolve;
            uniform sampler2D _DissolveNoise; uniform float4 _DissolveNoise_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_27 = i.uv0;
                float node_4 = (1.0 - saturate((tex2D(_DissolveNoise,TRANSFORM_TEX(node_27.rg, _DissolveNoise)).r+(_Dissolve*2.0+-1.0))));
                clip(node_4 - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = (_Glow.rgb*node_4*_Bloom);
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float _Dissolve;
            uniform sampler2D _DissolveNoise; uniform float4 _DissolveNoise_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float2 uv0 : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_28 = i.uv0;
                float node_4 = (1.0 - saturate((tex2D(_DissolveNoise,TRANSFORM_TEX(node_28.rg, _DissolveNoise)).r+(_Dissolve*2.0+-1.0))));
                clip(node_4 - 0.5);
                SHADOW_COLLECTOR_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float _Dissolve;
            uniform sampler2D _DissolveNoise; uniform float4 _DissolveNoise_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_29 = i.uv0;
                float node_4 = (1.0 - saturate((tex2D(_DissolveNoise,TRANSFORM_TEX(node_29.rg, _DissolveNoise)).r+(_Dissolve*2.0+-1.0))));
                clip(node_4 - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
