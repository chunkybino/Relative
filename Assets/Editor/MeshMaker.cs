using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MeshMaker : MonoBehaviour
{
    public Vector3[] vertices;
    public Vector2[] uv;

    public Vector3Int[] triangles;
    public int[] trianglesInt;

    public bool makeNew;
    public string makeNewName = "NewMesh";

    public Mesh mesh;

    public bool saveMesh;
    public bool readMesh;

    public bool saveTri;
    public bool readTri;
    //public bool doubleTri;
    //public bool reverseTri;

    public bool readVertex;

    public bool subdivide;

    void OnValidate()
    {
        if (makeNew) {
            makeNew = false;

            mesh = new Mesh();

            AssetDatabase.CreateAsset(mesh, "Assets/Meshes/"+makeNewName+".asset");
        }

        if (saveMesh) {
            saveMesh = false;
            if (!mesh) return;

            mesh.vertices = vertices;
            mesh.uv = uv;

            SetTriInt();
            mesh.triangles = trianglesInt;

            AssetDatabase.SaveAssets();
        }

        if (readMesh)
        {
            readMesh = false;
            ReadTri();
            ReadVertex();
            ReadUV();
        }

        if (saveTri)
        {
            saveTri = false;
            SetTriInt();
            mesh.triangles = trianglesInt;

            AssetDatabase.SaveAssets();
        }

        if (readTri)
        {
            readTri = false;
            ReadTri();
        }

        if (readVertex)
        {
            readVertex = false;
            ReadVertex();
        }

        if (subdivide)
        {
            subdivide = false;
            SubdivideMesh(ref vertices, ref uv, ref triangles);
        }

        void ReadTri()
        {
            triangles = new Vector3Int[mesh.triangles.Length / 3];
            for (int i = 0; i < triangles.Length; i++) {
                triangles[i] = new Vector3Int(mesh.triangles[3*i+0],mesh.triangles[3*i+1],mesh.triangles[3*i+2]);
            }
        }
        void ReadVertex()
        {
            vertices = mesh.vertices;
        }
        void ReadUV()
        {
            uv = mesh.uv;
        }
    }

    void SetTriInt()
    {
        trianglesInt = new int[triangles.Length * 3];

        for (int i = 0; i < triangles.Length; i++)
        {
            trianglesInt[i*3] = triangles[i].x;
            trianglesInt[i*3 + 1] = triangles[i].y;
            trianglesInt[i*3 + 2] = triangles[i].z;
        }
    }

    public static void SubdivideMesh(ref Vector3[] inVertex, ref Vector2[] inUV, ref int[] inTri)
    {
        Vector3Int[] triVec = GetTriVector(inTri);
        SubdivideMesh(ref inVertex, ref inUV, ref triVec);
        inTri = GetTriInt(triVec);
    }
    public static void SubdivideMesh(ref Vector3[] inVertex, ref Vector2[] inUV, ref Vector3Int[] inTri)
    {
        List<Vector3> newVertList = new List<Vector3>();
        Dictionary<Vector3,int> newVertDict = new Dictionary<Vector3,int>();

        List<Vector2> newUVList = new List<Vector2>();

        List<Vector3Int> newTriList = new List<Vector3Int>();        

        for (int i = 0; i < inTri.Length; i++)
        {
            Vector3 vertex1 = inVertex[inTri[i].x];
            Vector3 vertex2 = inVertex[inTri[i].y];
            Vector3 vertex3 = inVertex[inTri[i].z];

            Vector2 uv1 = inUV[inTri[i].x];
            Vector2 uv2 = inUV[inTri[i].y];
            Vector2 uv3 = inUV[inTri[i].z];

            Vector3 newVertex1 = Vector3.Lerp(vertex1,vertex2, 0.5f);
            Vector3 newVertex2 = Vector3.Lerp(vertex2,vertex3, 0.5f);
            Vector3 newVertex3 = Vector3.Lerp(vertex3,vertex1, 0.5f);

            Vector2 newUV1 = Vector2.Lerp(uv1,uv2, 0.5f);
            Vector2 newUV2 = Vector2.Lerp(uv2,uv3, 0.5f);
            Vector2 newUV3 = Vector2.Lerp(uv3,uv1, 0.5f);

            AddNewTris(
                new Vector3[] {
                    vertex1,vertex2,vertex3,
                    newVertex1,newVertex2,newVertex3
                },
                new Vector2[] {
                    uv1,uv2,uv3,
                    newUV1,newUV2,newUV3
                }
            );
        }

        inTri = UFunc.List2Array(newTriList);
        inUV = UFunc.List2Array(newUVList);
        inVertex = UFunc.List2Array(newVertList);

        void AddNewTris(Vector3[] triVertex, Vector2[] triUV)
        {
            int[] vertexIndex = new int[6];

            for (int i = 0; i < triVertex.Length; i++)
            {
                Vector3 v = triVertex[i];

                if (!newVertDict.ContainsKey(v))
                {
                    vertexIndex[i] = newVertList.Count;
                    newVertDict.Add(v,newVertList.Count);
                    newVertList.Add(v);
                    newUVList.Add(triUV[i]);
                }
                else
                {
                    vertexIndex[i] = newVertDict[v];
                }
            }

            newTriList.Add(new Vector3Int(vertexIndex[0],vertexIndex[3],vertexIndex[5]));
            newTriList.Add(new Vector3Int(vertexIndex[1],vertexIndex[4],vertexIndex[3]));
            newTriList.Add(new Vector3Int(vertexIndex[2],vertexIndex[5],vertexIndex[4]));
            newTriList.Add(new Vector3Int(vertexIndex[3],vertexIndex[4],vertexIndex[5]));
        }
    }

    public static Vector3Int[] GetTriVector(int[] triangles)
    {
        Vector3Int[] newTri = new Vector3Int[triangles.Length / 3];
        for (int i = 0; i < newTri.Length; i++) {
            newTri[i] = new Vector3Int(triangles[3*i+0],triangles[3*i+1],triangles[3*i+2]);
        }
        return newTri;
    }
    public static int[] GetTriInt(Vector3Int[] triangles)
    {
        int[] newTri = new int[triangles.Length*3];
        for (int i = 0; i < triangles.Length; i++) {
            newTri[3*i] = triangles[i].x;
            newTri[3*i+1] = triangles[i].y;
            newTri[3*i+2] = triangles[i].z;
        }
        return newTri;
    }
}
