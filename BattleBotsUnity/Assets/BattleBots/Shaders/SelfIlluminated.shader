// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|spec-289-OUT,normal-9-RGB,emission-427-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:2,x:34044,y:32568,ptlb:Cutout,ptin:_Cutout,glob:False,tex:edbd3e4d60628f24ea59b3e704152ede;n:type:ShaderForge.SFN_Tex2d,id:3,x:33898,y:32629,tex:edbd3e4d60628f24ea59b3e704152ede,ntxv:0,isnm:False|TEX-2-TEX;n:type:ShaderForge.SFN_Color,id:5,x:33919,y:32801,ptlb:Main Color,ptin:_MainColor,glob:False,c1:0.5297399,c2:0.4848616,c3:0.8676471,c4:1;n:type:ShaderForge.SFN_Color,id:8,x:33609,y:32477,ptlb:Secondary Color,ptin:_SecondaryColor,glob:False,c1:0.5514706,c2:0.5047131,c3:0.2432958,c4:1;n:type:ShaderForge.SFN_Tex2d,id:9,x:33189,y:33299,ptlb:Normal,ptin:_Normal,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Multiply,id:126,x:33475,y:32876|A-3-RGB,B-5-RGB,C-353-OUT;n:type:ShaderForge.SFN_Fresnel,id:152,x:33747,y:33139|EXP-162-OUT;n:type:ShaderForge.SFN_Vector1,id:162,x:34001,y:33173,v1:0.5;n:type:ShaderForge.SFN_OneMinus,id:202,x:33518,y:33109|IN-152-OUT;n:type:ShaderForge.SFN_OneMinus,id:230,x:33685,y:32629|IN-3-RGB;n:type:ShaderForge.SFN_Slider,id:289,x:33008,y:32630,ptlb:Spec,ptin:_Spec,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Cubemap,id:337,x:33210,y:32945,ptlb:Cubemap,ptin:_Cubemap,cube:240b912641cf3ad43b8b4a830fa06c29,pvfc:0;n:type:ShaderForge.SFN_Multiply,id:353,x:33496,y:33266|A-202-OUT,B-354-OUT;n:type:ShaderForge.SFN_Vector1,id:354,x:33680,y:33326,v1:2;n:type:ShaderForge.SFN_Add,id:410,x:33168,y:32720|A-411-OUT,B-126-OUT;n:type:ShaderForge.SFN_Multiply,id:411,x:33404,y:32591|A-8-RGB,B-230-OUT;n:type:ShaderForge.SFN_Multiply,id:427,x:32989,y:32805|A-410-OUT,B-337-RGB;proporder:5-8-289-2-337-9;pass:END;sub:END;*/

Shader "Shader Forge/SelfIlluminated" {
    Properties {
        _MainColor ("Main Color", Color) = (0.5297399,0.4848616,0.8676471,1)
        _SecondaryColor ("Secondary Color", Color) = (0.5514706,0.5047131,0.2432958,1)
        _Spec ("Spec", Range(0, 1)) = 0
        _Cutout ("Cutout", 2D) = "white" {}
        _Cubemap ("Cubemap", Cube) = "_Skybox" {}
        _Normal ("Normal", 2D) = "bump" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
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
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Cutout; uniform float4 _Cutout_ST;
            uniform float4 _MainColor;
            uniform float4 _SecondaryColor;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Spec;
            uniform samplerCUBE _Cubemap;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_437 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_437.rg, _Normal))).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
////// Emissive:
                float4 node_3 = tex2D(_Cutout,TRANSFORM_TEX(node_437.rg, _Cutout));
                float3 emissive = (((_SecondaryColor.rgb*(1.0 - node_3.rgb))+(node_3.rgb*_MainColor.rgb*((1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),0.5))*2.0)))*texCUBE(_Cubemap,viewReflectDirection).rgb);
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0.0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Spec,_Spec,_Spec);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                finalColor += specular;
                finalColor += emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Cutout; uniform float4 _Cutout_ST;
            uniform float4 _MainColor;
            uniform float4 _SecondaryColor;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Spec;
            uniform samplerCUBE _Cubemap;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_438 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_438.rg, _Normal))).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0.0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Spec,_Spec,_Spec);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
