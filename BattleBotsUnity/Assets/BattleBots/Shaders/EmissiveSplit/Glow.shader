// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-85-OUT,emission-15-OUT,alpha-330-OUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:34030,y:32687,ptlb:Lines,ptin:_Lines,tex:562a3a0b649db4345a735d31a454df73,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:4,x:33591,y:32450,ptlb:Base,ptin:_Base,glob:False,c1:0.8682613,c2:0.2941176,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:15,x:33313,y:32932|A-3-A,B-17-RGB;n:type:ShaderForge.SFN_Color,id:17,x:33664,y:32972,ptlb:Glow,ptin:_Glow,glob:False,c1:0,c2:1,c3:0.04827595,c4:1;n:type:ShaderForge.SFN_OneMinus,id:80,x:33604,y:32637|IN-3-A;n:type:ShaderForge.SFN_Multiply,id:85,x:33243,y:32599|A-4-RGB,B-80-OUT;n:type:ShaderForge.SFN_Add,id:330,x:33243,y:32771|A-4-A,B-3-A;proporder:4-3-17;pass:END;sub:END;*/

Shader "Shader Forge/Glow" {
    Properties {
        _Base ("Base", Color) = (0.8682613,0.2941176,1,1)
        _Lines ("Lines", 2D) = "white" {}
        _Glow ("Glow", Color) = (0,1,0.04827595,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Lines; uniform float4 _Lines_ST;
            uniform float4 _Base;
            uniform float4 _Glow;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb;
////// Emissive:
                float2 node_391 = i.uv0;
                float4 node_3 = tex2D(_Lines,TRANSFORM_TEX(node_391.rg, _Lines));
                float3 node_15 = (node_3.a*_Glow.rgb);
                float3 emissive = node_15;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float3 node_85 = (_Base.rgb*(1.0 - node_3.a));
                finalColor += diffuseLight * node_85;
                finalColor += emissive;
                float node_330 = (_Base.a+node_3.a);
/// Final Color:
                return fixed4(finalColor,node_330);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Lines; uniform float4 _Lines_ST;
            uniform float4 _Base;
            uniform float4 _Glow;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_392 = i.uv0;
                float4 node_3 = tex2D(_Lines,TRANSFORM_TEX(node_392.rg, _Lines));
                float3 node_85 = (_Base.rgb*(1.0 - node_3.a));
                finalColor += diffuseLight * node_85;
                float node_330 = (_Base.a+node_3.a);
/// Final Color:
                return fixed4(finalColor * node_330,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
