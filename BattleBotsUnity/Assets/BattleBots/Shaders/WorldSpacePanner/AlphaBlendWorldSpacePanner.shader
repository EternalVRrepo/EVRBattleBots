// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:0,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:True,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32582,y:32674|emission-464-OUT,alpha-529-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33409,y:32761,ptlb:Emission,ptin:_Emission,ntxv:2,isnm:False|UVIN-427-OUT;n:type:ShaderForge.SFN_Color,id:3,x:33409,y:32603,ptlb:EmissionTint,ptin:_EmissionTint,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:67,x:33181,y:32613|A-3-RGB,B-2-RGB;n:type:ShaderForge.SFN_ValueProperty,id:134,x:34414,y:33074,ptlb:USpeed,ptin:_USpeed,glob:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:135,x:34429,y:33156,ptlb:VSpeed,ptin:_VSpeed,glob:False,v1:0;n:type:ShaderForge.SFN_FragmentPosition,id:344,x:34103,y:32587;n:type:ShaderForge.SFN_ComponentMask,id:345,x:33942,y:32588,cc1:0,cc2:2,cc3:-1,cc4:-1|IN-344-XYZ;n:type:ShaderForge.SFN_TexCoord,id:347,x:34075,y:32967,uv:0;n:type:ShaderForge.SFN_Add,id:394,x:33901,y:33079|A-347-UVOUT,B-425-OUT;n:type:ShaderForge.SFN_Append,id:423,x:34253,y:33044|A-134-OUT,B-135-OUT;n:type:ShaderForge.SFN_Time,id:424,x:34253,y:33197;n:type:ShaderForge.SFN_Multiply,id:425,x:34075,y:33130|A-423-OUT,B-424-T;n:type:ShaderForge.SFN_Add,id:427,x:33579,y:32761|A-430-OUT,B-431-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:430,x:33767,y:32545,ptlb:WorldSpace,ptin:_WorldSpace,on:True|A-505-OUT,B-345-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:431,x:33746,y:33008,ptlb:Panner,ptin:_Panner,on:True|A-519-UVOUT,B-394-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:436,x:33026,y:32831,ptlb:useEmissionTintAlpha,ptin:_useEmissionTintAlpha,on:False|A-2-A,B-3-A;n:type:ShaderForge.SFN_Multiply,id:447,x:33230,y:33202|A-449-RGB,B-450-RGB;n:type:ShaderForge.SFN_Color,id:449,x:33559,y:33149,ptlb:DiffuseTint,ptin:_DiffuseTint,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:450,x:33559,y:33324,ptlb:Diffuse,ptin:_Diffuse,ntxv:0,isnm:False;n:type:ShaderForge.SFN_SwitchProperty,id:464,x:32823,y:32723,ptlb:useDiffuse,ptin:_useDiffuse,on:False|A-67-OUT,B-465-OUT;n:type:ShaderForge.SFN_Add,id:465,x:33035,y:33142|A-67-OUT,B-447-OUT;n:type:ShaderForge.SFN_Vector2,id:505,x:33927,y:32504,v1:0,v2:0;n:type:ShaderForge.SFN_TexCoord,id:519,x:33903,y:32938,uv:0;n:type:ShaderForge.SFN_SwitchProperty,id:529,x:32859,y:32954,ptlb:useDiffuseAlpha,ptin:_useDiffuseAlpha,on:False|A-436-OUT,B-450-A;proporder:431-430-464-3-2-449-450-436-529-134-135;pass:END;sub:END;*/

Shader "Shader Forge/Particle/AlphaBlendWorldSpacePanner" {
    Properties {
        [MaterialToggle] _Panner ("Panner", Float ) = 0
        [MaterialToggle] _WorldSpace ("WorldSpace", Float ) = 0.827451
        [MaterialToggle] _useDiffuse ("useDiffuse", Float ) = 0
        _EmissionTint ("EmissionTint", Color) = (0.5,0.5,0.5,1)
        _Emission ("Emission", 2D) = "black" {}
        _DiffuseTint ("DiffuseTint", Color) = (0.5,0.5,0.5,1)
        _Diffuse ("Diffuse", 2D) = "white" {}
        [MaterialToggle] _useEmissionTintAlpha ("useEmissionTintAlpha", Float ) = 1
        [MaterialToggle] _useDiffuseAlpha ("useDiffuseAlpha", Float ) = 1
        _USpeed ("USpeed", Float ) = 1
        _VSpeed ("VSpeed", Float ) = 0
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
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Emission; uniform float4 _Emission_ST;
            uniform float4 _EmissionTint;
            uniform float _USpeed;
            uniform float _VSpeed;
            uniform fixed _WorldSpace;
            uniform fixed _Panner;
            uniform fixed _useEmissionTintAlpha;
            uniform float4 _DiffuseTint;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform fixed _useDiffuse;
            uniform fixed _useDiffuseAlpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_424 = _Time + _TimeEditor;
                float2 node_427 = (lerp( float2(0,0), i.posWorld.rgb.rb, _WorldSpace )+lerp( i.uv0.rg, (i.uv0.rg+(float2(_USpeed,_VSpeed)*node_424.g)), _Panner ));
                float4 node_2 = tex2D(_Emission,TRANSFORM_TEX(node_427, _Emission));
                float3 node_67 = (_EmissionTint.rgb*node_2.rgb);
                float2 node_537 = i.uv0;
                float4 node_450 = tex2D(_Diffuse,TRANSFORM_TEX(node_537.rg, _Diffuse));
                float3 emissive = lerp( node_67, (node_67+(_DiffuseTint.rgb*node_450.rgb)), _useDiffuse );
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,lerp( lerp( node_2.a, _EmissionTint.a, _useEmissionTintAlpha ), node_450.a, _useDiffuseAlpha ));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
