using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GPUMarchingCubesDrawMesh : MonoBehaviour
{
    #region public
    public int segmentNum = 32;

    [Range(0, 1)]
    public float threshold = 0.5f;
    public Material mat;

    public Color DiffuseColor = Color.green;
    public Color EmissionColor = Color.black;
    public float EmissionIntensity = 0;

    [Range(0, 1)]
    public float metallic = 0;
    [Range(0, 1)]
    public float glossiness = 0.5f;
    #endregion

    #region private
    int vertexMax = 0;
    Mesh[] meshes = null;
    Material[] materials = null;
    float renderScale = 1f / 32f;

    MarchingCubesDefines mcDefines = null;
    #endregion

    void Initialize()
    {
        vertexMax = segmentNum * segmentNum * segmentNum;
        Debug.Log("vertexMax : " + vertexMax);

        renderScale = 1f / segmentNum;
        CreateMesh();
            
        mcDefines = new MarchingCubesDefines();
    }

    void CreateMesh()
    {
        // 最大頂点数に合わせてメッシュを分割(この場合は5).
        int vertNum = 65535;
        int meshNum = Mathf.CeilToInt((float)vertexMax / vertNum);
        Debug.Log("meshNum : " + meshNum);

        meshes = new Mesh[meshNum];
        materials = new Material[meshNum];

        Bounds bounds = new Bounds
        (
            transform.position,
            new Vector3(segmentNum, segmentNum, segmentNum) * renderScale
        );

        int id = 0;
        for (int i = 0; i < meshNum; i++)
        {
            // 頂点を作成.
            Vector3[] vertices = new Vector3[vertNum];
            int[] indices = new int[vertNum];
            for (int j = 0; j < vertNum; j++)
            {
                vertices[j].x = (id % segmentNum);
                vertices[j].y = ((id / segmentNum) % segmentNum);
                vertices[j].z = ((id / (segmentNum * segmentNum)) % segmentNum);

                indices[j] = j;
                id++;
            }
            // meshを作成.
            meshes[i] = new Mesh();
            meshes[i].vertices = vertices;
            meshes[i].SetIndices(indices, MeshTopology.Points, 0);
            meshes[i].bounds = bounds;

            materials[i] = new Material(mat);
        }
    }

    void RenderMesh()
    {
        Vector3 halfSize = new Vector3(segmentNum, segmentNum, segmentNum) * renderScale * 0.5f;
        Matrix4x4 trs = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        for (int i = 0; i < meshes.Length; i++)
        {
            materials[i].SetPass(0);

            materials[i].SetInt("_SegmentNum", segmentNum);
            materials[i].SetFloat("_Scale", renderScale);
            materials[i].SetFloat("_Threshold", threshold);
            materials[i].SetFloat("_Metallic", metallic);
            materials[i].SetFloat("_Glossiness", glossiness);
            materials[i].SetFloat("_EmissionIntensity", EmissionIntensity);

            materials[i].SetVector("_HalfSize", halfSize);
            materials[i].SetColor("_DiffuseColor", DiffuseColor);
            materials[i].SetColor("_EmissionColor", EmissionColor);
            materials[i].SetMatrix("_Matrix", trs);

            materials[i].SetBuffer("vertexOffset", mcDefines.vertexOffsetBuffer);
            materials[i].SetBuffer("cubeEdgeFlags", mcDefines.cubeEdgeFlagsBuffer);
            materials[i].SetBuffer("edgeConnection", mcDefines.edgeConnectionBuffer);
            materials[i].SetBuffer("edgeDirection", mcDefines.edgeDirectionBuffer);
            materials[i].SetBuffer("triangleConnectionTable", mcDefines.triangleConnectionTableBuffer);
            // 即座に描画せず，レンダリング処理に登録するので，Update()内で呼び出す.
            Graphics.DrawMesh(meshes[i], Matrix4x4.identity, materials[i], 0);
        }
    }

    private void Start()
    {
        // meshを生成.
        Initialize();
    }

    private void Update()
    {
        // レンダリング.
        RenderMesh();
    }

    private void OnDestroy()
    {
        mcDefines.ReleaseBuffer();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
