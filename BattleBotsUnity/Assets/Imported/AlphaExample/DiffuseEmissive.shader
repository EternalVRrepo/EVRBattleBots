// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|normal-176-RGB,emission-169-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33607,y:32511,ptlb:Diffuse,ptin:_Diffuse,tex:03852077a9c2f9f4f806d2b2ee4240da,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:8,x:33619,y:32938,ptlb:Emissive Tint,ptin:_EmissiveTint,glob:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:49,x:33410,y:32836|A-2-A,B-8-RGB;n:type:ShaderForge.SFN_OneMinus,id:167,x:33410,y:32710|IN-2-A;n:type:ShaderForge.SFN_Add,id:168,x:33255,y:32774|A-167-OUT,B-49-OUT;n:type:ShaderForge.SFN_Multiply,id:169,x:33108,y:32721|A-2-RGB,B-168-OUT;n:type:ShaderForge.SFN_Tex2d,id:176,x:33043,y:32335,ptlb:Normal,ptin:_Normal,tex:163999d8dab5191488f626d743b15ac7,ntxv:2,isnm:False;proporder:8-2-176;pass:END;sub:END;*/

Shader "Shader Forge/DiffuseEmissive" {
    Properties {
        _EmissiveTint ("Emissive Tint", Color) = (1,0,0,1)
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Normal ("Normal", 2D) = "black" {}
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _EmissiveTint;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 binormalDir : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
/////// Normals:
                float2 node_203 = i.uv0;
                float3 normalLocal = tex2D(_Normal,TRANSFORM_TEX(node_203.rg, _Normal)).rgb;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
////// Lighting:
////// Emissive:
                float4 node_2 = tex2D(_Diffuse,TRANSFORM_TEX(node_203.rg, _Diffuse));
                float3 emissive = (node_2.rgb*((1.0 - node_2.a)+(node_2.a*_EmissiveTint.rgb)));
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
