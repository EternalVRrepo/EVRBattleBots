// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:30611,y:31542|emission-393-OUT,alpha-395-OUT;n:type:ShaderForge.SFN_Floor,id:2,x:33326,y:31701,cmnt:p|IN-151-OUT;n:type:ShaderForge.SFN_Frac,id:3,x:33463,y:31960|IN-151-OUT;n:type:ShaderForge.SFN_Multiply,id:5,x:33260,y:32087|A-3-OUT,B-8-OUT;n:type:ShaderForge.SFN_Vector1,id:6,x:33273,y:31994,v1:3;n:type:ShaderForge.SFN_Vector1,id:8,x:33463,y:32105,v1:2;n:type:ShaderForge.SFN_Subtract,id:9,x:33094,y:32004|A-6-OUT,B-5-OUT;n:type:ShaderForge.SFN_Multiply,id:10,x:32202,y:31870,cmnt:f|A-3-OUT,B-3-OUT,C-9-OUT;n:type:ShaderForge.SFN_ComponentMask,id:11,x:33166,y:31648,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-2-OUT;n:type:ShaderForge.SFN_Vector1,id:12,x:33155,y:31828,v1:57;n:type:ShaderForge.SFN_Multiply,id:13,x:32956,y:31709|A-11-G,B-12-OUT;n:type:ShaderForge.SFN_Add,id:14,x:32806,y:31641,cmnt:n|A-11-R,B-13-OUT;n:type:ShaderForge.SFN_Sin,id:16,x:32233,y:31205|IN-17-OUT;n:type:ShaderForge.SFN_Add,id:17,x:32602,y:31210|A-14-OUT,B-21-OUT;n:type:ShaderForge.SFN_Add,id:18,x:32584,y:31345|A-14-OUT,B-22-OUT;n:type:ShaderForge.SFN_Add,id:19,x:32609,y:31489|A-14-OUT,B-23-OUT;n:type:ShaderForge.SFN_Vector1,id:21,x:32850,y:31266,v1:58;n:type:ShaderForge.SFN_Vector1,id:22,x:32832,y:31405,v1:57;n:type:ShaderForge.SFN_Vector1,id:23,x:32791,y:31515,v1:1;n:type:ShaderForge.SFN_Multiply,id:24,x:32023,y:31193|A-25-OUT,B-16-OUT;n:type:ShaderForge.SFN_Vector1,id:25,x:32233,y:31078,v1:473.5854;n:type:ShaderForge.SFN_Multiply,id:27,x:32023,y:31349|A-25-OUT,B-29-OUT;n:type:ShaderForge.SFN_Sin,id:29,x:32233,y:31360|IN-18-OUT;n:type:ShaderForge.SFN_Multiply,id:31,x:32023,y:31510|A-25-OUT,B-33-OUT;n:type:ShaderForge.SFN_Sin,id:33,x:32233,y:31521|IN-19-OUT;n:type:ShaderForge.SFN_Multiply,id:35,x:32023,y:31657|A-25-OUT,B-37-OUT;n:type:ShaderForge.SFN_Sin,id:37,x:32233,y:31668|IN-14-OUT;n:type:ShaderForge.SFN_Frac,id:38,x:31814,y:31193|IN-24-OUT;n:type:ShaderForge.SFN_Frac,id:40,x:31814,y:31336|IN-27-OUT;n:type:ShaderForge.SFN_Frac,id:42,x:31814,y:31510|IN-31-OUT;n:type:ShaderForge.SFN_Frac,id:44,x:31814,y:31657|IN-35-OUT;n:type:ShaderForge.SFN_Lerp,id:45,x:31481,y:31300|A-40-OUT,B-38-OUT,T-48-R;n:type:ShaderForge.SFN_Lerp,id:46,x:31513,y:31488|A-44-OUT,B-42-OUT,T-48-R;n:type:ShaderForge.SFN_Lerp,id:47,x:31328,y:31386|A-46-OUT,B-45-OUT,T-48-G;n:type:ShaderForge.SFN_ComponentMask,id:48,x:31886,y:31857,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-10-OUT;n:type:ShaderForge.SFN_Multiply,id:151,x:33619,y:31874|A-378-OUT,B-152-OUT;n:type:ShaderForge.SFN_Vector1,id:152,x:34072,y:32023,cmnt:frequency,v1:10;n:type:ShaderForge.SFN_Vector2,id:197,x:34040,y:31553,cmnt:move,v1:0.1,v2:0.1;n:type:ShaderForge.SFN_TexCoord,id:198,x:33939,y:31814,uv:0;n:type:ShaderForge.SFN_Time,id:200,x:34040,y:31673;n:type:ShaderForge.SFN_Multiply,id:201,x:33841,y:31623|A-197-OUT,B-200-T;n:type:ShaderForge.SFN_Vector1,id:203,x:31304,y:31562,cmnt:amplitude,v1:1;n:type:ShaderForge.SFN_Multiply,id:291,x:31151,y:32400,cmnt:a single octave of noise|A-295-OUT,B-293-OUT;n:type:ShaderForge.SFN_Vector1,id:293,x:31400,y:32609,cmnt:amplitude,v1:1;n:type:ShaderForge.SFN_Lerp,id:295,x:31400,y:32387|A-299-OUT,B-297-OUT,T-301-G;n:type:ShaderForge.SFN_Lerp,id:297,x:31553,y:32301|A-307-OUT,B-309-OUT,T-301-R;n:type:ShaderForge.SFN_Lerp,id:299,x:31585,y:32489|A-303-OUT,B-305-OUT,T-301-R;n:type:ShaderForge.SFN_ComponentMask,id:301,x:31958,y:32858,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-319-OUT;n:type:ShaderForge.SFN_Frac,id:303,x:31886,y:32658|IN-317-OUT;n:type:ShaderForge.SFN_Frac,id:305,x:31886,y:32511|IN-315-OUT;n:type:ShaderForge.SFN_Frac,id:307,x:31886,y:32337|IN-313-OUT;n:type:ShaderForge.SFN_Frac,id:309,x:31886,y:32194|IN-311-OUT;n:type:ShaderForge.SFN_Multiply,id:311,x:32095,y:32194|A-329-OUT,B-327-OUT;n:type:ShaderForge.SFN_Multiply,id:313,x:32095,y:32350|A-329-OUT,B-325-OUT;n:type:ShaderForge.SFN_Multiply,id:315,x:32095,y:32511|A-329-OUT,B-323-OUT;n:type:ShaderForge.SFN_Multiply,id:317,x:32095,y:32658|A-329-OUT,B-321-OUT;n:type:ShaderForge.SFN_Multiply,id:319,x:32274,y:32871,cmnt:f|A-355-OUT,B-355-OUT,C-347-OUT;n:type:ShaderForge.SFN_Sin,id:321,x:32305,y:32669|IN-343-OUT;n:type:ShaderForge.SFN_Sin,id:323,x:32305,y:32522|IN-331-OUT;n:type:ShaderForge.SFN_Sin,id:325,x:32305,y:32361|IN-333-OUT;n:type:ShaderForge.SFN_Sin,id:327,x:32305,y:32206|IN-335-OUT;n:type:ShaderForge.SFN_Vector1,id:329,x:32305,y:32079,v1:473.5854;n:type:ShaderForge.SFN_Add,id:331,x:32681,y:32490|A-343-OUT,B-341-OUT;n:type:ShaderForge.SFN_Add,id:333,x:32656,y:32346|A-343-OUT,B-339-OUT;n:type:ShaderForge.SFN_Add,id:335,x:32674,y:32211|A-343-OUT,B-337-OUT;n:type:ShaderForge.SFN_Vector1,id:337,x:32922,y:32267,v1:58;n:type:ShaderForge.SFN_Vector1,id:339,x:32904,y:32406,v1:57;n:type:ShaderForge.SFN_Vector1,id:341,x:32863,y:32516,v1:1;n:type:ShaderForge.SFN_Add,id:343,x:32878,y:32642,cmnt:n|A-349-R,B-345-OUT;n:type:ShaderForge.SFN_Multiply,id:345,x:33028,y:32710|A-349-G,B-351-OUT;n:type:ShaderForge.SFN_Subtract,id:347,x:33166,y:33005|A-359-OUT,B-357-OUT;n:type:ShaderForge.SFN_ComponentMask,id:349,x:33238,y:32649,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-353-OUT;n:type:ShaderForge.SFN_Vector1,id:351,x:33227,y:32829,v1:57;n:type:ShaderForge.SFN_Floor,id:353,x:33398,y:32702,cmnt:p|IN-363-OUT;n:type:ShaderForge.SFN_Frac,id:355,x:33535,y:32961|IN-363-OUT;n:type:ShaderForge.SFN_Multiply,id:357,x:33332,y:33088|A-355-OUT,B-361-OUT;n:type:ShaderForge.SFN_Vector1,id:359,x:33345,y:32995,v1:3;n:type:ShaderForge.SFN_Vector1,id:361,x:33535,y:33106,v1:2;n:type:ShaderForge.SFN_Multiply,id:363,x:33691,y:32875|A-365-OUT,B-375-OUT;n:type:ShaderForge.SFN_Add,id:365,x:33727,y:32697|A-369-OUT,B-367-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:367,x:33965,y:32823,uv:0;n:type:ShaderForge.SFN_Multiply,id:369,x:33913,y:32624|A-371-OUT,B-373-T;n:type:ShaderForge.SFN_Vector2,id:371,x:34112,y:32554,cmnt:move,v1:0.1,v2:0;n:type:ShaderForge.SFN_Time,id:373,x:34112,y:32674;n:type:ShaderForge.SFN_Vector1,id:375,x:34144,y:33024,cmnt:frequency,v1:4;n:type:ShaderForge.SFN_Add,id:378,x:33570,y:31605|A-201-OUT,B-380-OUT;n:type:ShaderForge.SFN_Multiply,id:380,x:33763,y:32019|A-198-UVOUT,B-291-OUT;n:type:ShaderForge.SFN_Multiply,id:391,x:31048,y:31572|A-47-OUT,B-203-OUT;n:type:ShaderForge.SFN_Color,id:392,x:31137,y:31318,ptlb:tin,ptin:_tin,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:393,x:30942,y:31347|A-392-RGB,B-391-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:395,x:30971,y:31764,ptlb:useAlpha,ptin:_useAlpha,on:False|A-396-OUT,B-391-OUT;n:type:ShaderForge.SFN_Vector1,id:396,x:31333,y:31797,v1:1;proporder:392-395;pass:END;sub:END;*/

Shader "Shader Forge/ValueNoise" {
    Properties {
        _tin ("tin", Color) = (0.5,0.5,0.5,1)
        [MaterialToggle] _useAlpha ("useAlpha", Float ) = 1
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
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _tin;
            uniform fixed _useAlpha;
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
////// Lighting:
////// Emissive:
                float node_25 = 473.5854;
                float4 node_200 = _Time + _TimeEditor;
                float node_329 = 473.5854;
                float4 node_373 = _Time + _TimeEditor;
                float2 node_363 = (((float2(0.1,0)*node_373.g)+i.uv0.rg)*4.0);
                float2 node_349 = floor(node_363).rg;
                float node_343 = (node_349.r+(node_349.g*57.0)); // n
                float2 node_355 = frac(node_363);
                float2 node_301 = (node_355*node_355*(3.0-(node_355*2.0))).rg;
                float2 node_151 = (((float2(0.1,0.1)*node_200.g)+(i.uv0.rg*(lerp(lerp(frac((node_329*sin(node_343))),frac((node_329*sin((node_343+1.0)))),node_301.r),lerp(frac((node_329*sin((node_343+57.0)))),frac((node_329*sin((node_343+58.0)))),node_301.r),node_301.g)*1.0)))*10.0);
                float2 node_11 = floor(node_151).rg;
                float node_14 = (node_11.r+(node_11.g*57.0)); // n
                float2 node_3 = frac(node_151);
                float2 node_48 = (node_3*node_3*(3.0-(node_3*2.0))).rg;
                float node_391 = (lerp(lerp(frac((node_25*sin(node_14))),frac((node_25*sin((node_14+1.0)))),node_48.r),lerp(frac((node_25*sin((node_14+57.0)))),frac((node_25*sin((node_14+58.0)))),node_48.r),node_48.g)*1.0);
                float3 emissive = (_tin.rgb*node_391);
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,lerp( 1.0, node_391, _useAlpha ));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
