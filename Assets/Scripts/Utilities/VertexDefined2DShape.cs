#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class VertexDefined2DShape : MonoBehaviour
{

    public List<Vector2> ShapeVertecies;
    public List<Vector2> ShapeUVCoords;
    public List<int> ShapeTriangles;
    public string MeshName = "VectorShapeMesh";
    public Material ShapeMaterial;

    public bool GenerateShapeNow;
    public bool CreateTriangle = false;
    public bool UseXY = false;
    public float PreScale = 1f;

    // Use this for initialization
    void Start()
    {
        Create2DShape();
    }

    void OnDrawGizmosSelected()
    {
        if (GenerateShapeNow)
        {
            Create2DShape();

            Mesh shapeMesh = GetComponent<MeshFilter>().sharedMesh;
            if (!AssetDatabase.Contains(shapeMesh))
            {
                Mesh temp = Instantiate(shapeMesh);
                string assetName = temp.name;
                if (temp.name.IndexOf("(Clone)") > 0)
                    assetName = temp.name.Substring(0, temp.name.IndexOf("(Clone)"));
                AssetDatabase.CreateAsset(temp, "Assets/GeneratedMeshes/" + assetName + ".asset");
                AssetDatabase.SaveAssets();
            }
            GenerateShapeNow = false;
        }
    }

    List<Vector2> CalcTrianglePoints(float sideLength)
    {
        List<Vector2> trianglePoints = new List<Vector2>(3);
        Vector2 centroid = Vector2.zero;

        //gereate first point by rotating unit vector by 60 degrees
        trianglePoints.Add(new Vector2(Mathf.Cos(Mathf.Deg2Rad * 60),
                                         Mathf.Sin(Mathf.Deg2Rad * 60)) * sideLength);

        //last 2 points are a simple horizontal line 
        trianglePoints.Add(Vector2.right * sideLength);
        trianglePoints.Add(Vector2.zero);

        //get the centrepoint of the triangle
        foreach (Vector2 point in trianglePoints)
            centroid += point;
        centroid /= 3;
        //transform verticies to be centred and scaled
        for (int i = 0; i < trianglePoints.Count; i++)
        {
            trianglePoints[i] -= centroid;
        }

        return trianglePoints;
    }

    void Create2DShape()
    {
        if (CreateTriangle)
            ShapeVertecies = CalcTrianglePoints(PreScale);

        PolygonCollider2D mCollider = GetComponent<PolygonCollider2D>();
        if (mCollider == null)
            mCollider = gameObject.AddComponent<PolygonCollider2D>();

        mCollider.points = ShapeVertecies.ToArray();

        MeshRenderer mRenderer = GetComponent<MeshRenderer>();
        if (mRenderer == null)
        {
            mRenderer = gameObject.AddComponent<MeshRenderer>();
            mRenderer.material = ShapeMaterial;
        }

        MeshFilter mFilter = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();

        //mesh must be defined in 3D, just leave z as 0 for 2D

        //Create the mesh
        mFilter.mesh = CreateMesh(MeshName,
                                   ShapeVertecies.Select(point => new Vector3(point.x, point.y, 0)).ToArray(),
                                   ShapeUVCoords.ToArray(),
                                   ShapeTriangles.ToArray());
    }

    Mesh CreateMesh(string meshName, Vector3[] verts, Vector2[] uv, int[] tris)
    {
        Mesh shapeMesh = new Mesh();
        shapeMesh.name = meshName;

        shapeMesh.vertices = verts;
        shapeMesh.uv = uv;
        shapeMesh.triangles = tris;

        shapeMesh.RecalculateNormals();

        return shapeMesh;
    }
}
#endif