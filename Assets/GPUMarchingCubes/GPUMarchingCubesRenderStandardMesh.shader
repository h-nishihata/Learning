﻿Shader "Custom/GPUMarchingCubesRenderStandardMesh"
{
    Properties
    {
        _SegmentNum("SegmentNum", int) = 32

        _Scale("Scale", float) = 1
        _Threshold("Threshold", float) = 0.5

        _DiffuseColor("Diffuse", Color) = (0, 0, 0, 1)

        _EmissionIntensitiy("Emission Intensity", Range(0, 1)) = 1
        _EmissionColor("Emission", Color) = (0, 0, 0, 1)

        _Glossiness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGINCLUDE
        #define UNITY_PASS_DEFERRED
        #include "HLSLSupport.cginc"
        #include "UnityShaderVariables.cginc"
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "UnityPBSLighting.cginc"

        #include "Libs/Primitives.cginc"
        #include "Libs/Utils.cginc"
        // レンダリングで使う構造体の定義.
        struct appdata
        {
            float4 vertex : POSITION; // POSITION = 頂点の位置.
        };

        struct v2g
        {
            float4 pos : SV_POSITION; // SV_POSITION = 頂点のクリップ座標.
        };

        struct g2f_light
        {
            float4 pos : SV_POSITION;
            float3 normal : NORMAL;
            float4 worldPos : TEXCOORD0;
            half3 sh : TEXCOORD3;
        };

        struct g2f_shadow
        {
            float4 pos : SV_POSITION;
            float4 hpos : TEXCOORD1;
        };

        int _SegmentNum;

        float _Scale;
        float _Threshold;

        float4 _DiffuseColor;
        float3 _HalfSize;
        float4x4 _Matrix;

        float _EmissionIntensity;
        half3 _EmissionColor;

        half _Glossiness;
        half _Metallic;

        StructuredBuffer<float3> vertexOffset;
        StructuredBuffer<int> cubeEdgeFlags;
        StructuredBuffer<int2> edgeConnection;
        StructuredBuffer<float3> edgeDirection;
        StructuredBuffer<int> triangleConnectionTable;

        // ボリュームデータから指定した座標のスカラー値を算出・取得する関数.
        float Sample(float x, float y, float z)
        {
            if ((x <= 1) ||
                (y <= 1) ||
                (z <= 1) ||
                (x >= (_SegmentNum - 1)) ||
                (y >= (_SegmentNum - 1)) ||
                (z >= (_SegmentNum - 1))
                )
                return 0;

            float3 size = float3(_SegmentNum, _SegmentNum, _SegmentNum);
            float3 pos = float3(x, y, z) / size;
            float result = 0;

            // 距離関数を使って3次元形状を定義.
            for (int i = 0; i < 3; i++)
            {
                // sphere(pos, rad);で閾値が0.5以上のときにポリゴン化する.
                float sp = -sphere(pos - float3(0.5, 0.25 + 0.25 * i, 0.5), 0.005 + (sin(_Time.y * 8.0 + i * 23.365) * 0.5 + 0.5) * 0.125) + 0.5;
                // smoothMax()でメタボール化する.
                result = smoothMax(result, sp, 14);
            }

            return result;
        }

        float getOffset(float val1, float val2, float desired)
        {
            float delta = val2 - val1;
            if (delta == 0.0)
            {
                return 0.5;
            }
            return (desired - val1) / delta;
        }

        float3 getNormal(float fX, float fY, float fZ)
        {
            float3 normal;
            float offset = 1.0;

            normal.x = Sample(fX - offset, fY, fZ) - Sample(fX + offset, fY, fZ);
            normal.y = Sample(fX, fY - offset, fZ) - Sample(fX, fY + offset, fZ);
            normal.z = Sample(fX, fY, fZ - offset) - Sample(fX, fY, fZ + offset);

            return normal;
        }

        // 頂点シェーダ. メッシュから渡される頂点データをそのままジオメトリシェーダに渡すだけ.
        v2g vert(appdata v)
        {
            v2g o = (v2g)0;
            o.pos = v.vertex;
            return o;
        }

        [maxvertexcount(15)]
        void geom_light(point v2g input[1], inout TriangleStream<g2f_light> outStream)
        {
            g2f_light o = (g2f_light)0;

            int i, j;
            float cubeValue[8];

            float3 edgeVertices[12] =
            {
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0)
            };

            float3 edgeNormals[12] =
            {
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0)
            };

            float3 pos = input[0].pos.xyz;
            float3 defpos = pos;
            // 1グリッドの8個の角の座標のボリュームデータ中のスカラー値を取得.
            for (i = 0; i < 8; i++)
            {
                cubeValue[i] = Sample
                (
                    pos.x + vertexOffset[i].x,
                    pos.y + vertexOffset[i].y,
                    pos.z + vertexOffset[i].z
                );
            }

            pos *= _Scale;
            pos -= _HalfSize;

            int flagIndex = 0;

            for (i = 0; i < 8; i++)
            {
                // グリッドの角のスカラー値が閾値を超えていたらフラグを立てる.
                if (cubeValue[i] <= _Threshold)
                {
                    flagIndex |= (1 << i);
                }
            }

            int edgeFlags = cubeEdgeFlags[flagIndex];

            if ((edgeFlags == 0) || (edgeFlags == 255))
            {
                return;
            }

            float offset = 0.5;
            float3 vertex;
            for (i = 0; i < 12; i++)
            {
                if ((edgeFlags & (1 << i)) != 0)
                {
                    offset = getOffset(cubeValue[edgeConnection[i].x], cubeValue[edgeConnection[i].y], _Threshold);
                    vertex = (vertexOffset[edgeConnection[i].x] + offset * edgeDirection[i]);

                    edgeVertices[i].x = pos.x + vertex.x * _Scale;
                    edgeVertices[i].y = pos.y + vertex.y * _Scale;
                    edgeVertices[i].z = pos.z + vertex.z * _Scale;

                    edgeNormals[i] = getNormal(defpos.x + vertex.x, defpos.y + vertex.y, defpos.z + vertex.z);
                }
            }

            int vindex = 0;
            int findex = 0;

            for (i = 0; i < 5; i++)
            {
                findex = flagIndex * 16 + 3 * i;
                if (triangleConnectionTable[findex] < 0)
                    break;

                for (j = 0; j < 3; j++)
                {
                    vindex = triangleConnectionTable[findex + j];

                    float4 ppos = mul(_Matrix, float4(edgeVertices[vindex], 1));
                    o.pos = UnityObjectToClipPos(ppos);

                    float3 norm = UnityObjectToWorldNormal(normalize(edgeNormals[vindex]));
                    o.normal = normalize(mul(_Matrix, float4(norm, 0)));

                    outStream.Append(o);
                }
                outStream.RestartStrip();
            }
        }

        void frag_light(g2f_light IN,
            out half4 outDiffuse        : SV_Target0,
            out half4 outSpecSmoothness : SV_Target1,
            out half4 outNormal         : SV_Target2,
            out half4 outEmission       : SV_Target3)
        {
            fixed3 normal = IN.normal;
            float3 worldPos = IN.worldPos;
            fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

#ifdef  UNITY_COMPILER_HLSL
            SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
            SurfaceOutputStandard o;
#endif
            o.Albedo = _DiffuseColor.rgb;
            o.Emission = _EmissionColor * _EmissionIntensity;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1.0;
            o.Occlusion = 1.0;
            o.Normal = normal;

            UnityGI gi;
            UNITY_INITIALIZE_OUTPUT(UnityGI, gi);

            UnityGIInput giInput;
            UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
            giInput.light = gi.light;
            giInput.worldPos = worldPos;
            giInput.worldViewDir = worldViewDir;
            giInput.atten = 1.0;

            giInput.ambient = IN.sh;

            giInput.probeHDR[0] = unity_SpecCube0_HDR;
            giInput.probeHDR[1] = unity_SpecCube1_HDR;

#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
            giInput.boxMin[0] = unity_SpecCube0_BoxMin;
#endif

#if UNITY_SPECCUBE_BOX_PROJECTION
            giInput.boxMax[0] = unity_SpecCube0_BoxMax;
            giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
            giInput.boxMax[1] = unity_SpecCube1_BoxMax;
            giInput.boxMin[1] = unity_SpecCube1_BoxMin;
            giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
#endif
            LightingStandard_GI(o, giInput, gi);

            outEmission = LightingStandard_Deferred(o, worldViewDir, gi, outDiffuse, outSpecSmoothness, outNormal);
            outDiffuse.a = 1.0;

#ifdef UNTIY_HDR_ON
            outEmmission.rgb = exp2(-outEmission.rgb);
#endif
        }

        [maxvertexcount(15)]
        void geom_shadow(point v2g input[1], inout TriangleStream<g2f_shadow> outStream)
        {
            g2f_shadow o = (g2f_shadow)0;

            int i, j;
            float cubeValue[8];

            float3 edgeVertices[12] =
            {
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0)
            };

            float3 edgeNormals[12] =
            {
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0),
                float3(0, 0, 0)
            };

            float3 pos = input[0].pos.xyz;
            float3 defpos = pos;

            for (i = 0; i < 8; i++)
            {
                cubeValue[i] = Sample
                (
                    pos.x + vertexOffset[i].x,
                    pos.y + vertexOffset[i].y,
                    pos.z + vertexOffset[i].z
                );
            }

            pos *= _Scale;
            pos -= _HalfSize;

            int flagIndex = 0;

            for (i = 0; i < 8; i++)
            {
                if (cubeValue[i] <= _Threshold)
                {
                    flagIndex |= (1 << i);
                }
            }

            int edgeFlags = cubeEdgeFlags[flagIndex];

            if ((edgeFlags == 0) || (edgeFlags == 255))
            {
                return;
            }

            float offset = 0.5;
            float3 vertex;
            for (i = 0; i < 12; i++)
            {
                if ((edgeFlags & (1 << i)) != 0)
                {
                    offset = getOffset(cubeValue[edgeConnection[i].x], cubeValue[edgeConnection[i].y], _Threshold);
                    vertex = (vertexOffset[edgeConnection[i].x] + offset * edgeDirection[i]);

                    edgeVertices[i].x = pos.x + vertex.x * _Scale;
                    edgeVertices[i].y = pos.y + vertex.y * _Scale;
                    edgeVertices[i].z = pos.z + vertex.z * _Scale;

                    edgeNormals[i] = getNormal(defpos.x + vertex.x, defpos.y + vertex.y, defpos.z + vertex.z);
                }
            }

            int vindex = 0;
            int findex = 0;

            for (i = 0; i < 5; i++)
            {
                findex = flagIndex * 16 + 3 * i;
                if (triangleConnectionTable[findex] < 0)
                    break;

                for (j = 0; j < 3; j++)
                {
                    vindex = triangleConnectionTable[findex + j];

                    float4 ppos = mul(_Matrix, float4(edgeVertices[vindex], 1));

                    float3 norm;
                    norm = UnityObjectToWorldNormal(normalize(edgeNormals[vindex]));
                    
                    float4 lpos1 = mul(unity_WorldToObject, ppos);
                    o.pos = UnityClipSpaceShadowCasterPos(lpos1, normalize(mul(_Matrix, float4(norm, 0))));
                    o.pos = UnityApplyLinearShadowBias(o.pos);
                    o.hpos = o.pos;

                    outStream.Append(o);
                }
                outStream.RestartStrip();
            }
        }

        fixed4 frag_shadow(g2f_shadow i) : SV_Target
        {
            return i.hpos.z / i.hpos.w;
        }
        ENDCG

        Pass{
            Tags{"LightMode" = "Deferred"}

                CGPROGRAM
                #pragma target 5.0
                #pragma vertex vert
                #pragma geometry geom_light
                #pragma fragment frag_light
                #pragma exclude_renderers nomrt
                #pragma multi_compile_prepassfinal noshadow
                ENDCG
        }

        Pass{
            Tags{"LightMode" = "ShadowCaster"}
            ZWrite On ZTest LEqual
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma geometry geom_shadow
            #pragma fragment frag_shadow
            #pragma multi_compile_shadowcaster
            ENDCG
        }
    }

    FallBack "Diffuse"
}
