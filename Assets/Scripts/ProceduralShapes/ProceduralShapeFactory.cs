using System;
using System.Collections.Generic;
using System.Linq;
using ProceduralShapes;
using UnityEngine;

public class ProceduralShapeFactory : MonoBehaviour
{

    [Range(0, 5)]
    public int Subdivisions = 4;
    public float Size = 1;
    public bool Truncate;
    public bool SmoothShaded;
    public Material DefaultMaterial;

    [Serializable]
    public enum Shape { Icosphere, Cube }
    public Shape shape = Shape.Icosphere;

    public static ProceduralShapeFactory Instance;

    void Awake() { Instance = this; }

    public void CreateNewCubeObject()
    {
        ProceduralCube shape = new ProceduralCube(Subdivisions);

        shape.GenerateUVs();

        Mesh mesh = new Mesh
        {
            name = "Cube_Mesh",
            vertices = shape.Vertices.Select(v => v.Position).ToArray(),
            uv = shape.Vertices.Select(v => v.UV).ToArray(),
            triangles = ProceduralShape.TriFace.TriFaceListToIndexArray(shape.TriFaces),
        };
        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        GameObject cube = new GameObject("Cube", typeof(MeshFilter), typeof(MeshRenderer));
        cube.GetComponent<MeshFilter>().sharedMesh = mesh;
        cube.GetComponent<MeshRenderer>().material = DefaultMaterial;
    }

    public void CreateNewIcosphereObject()
    {
        ProceduralIcosphere icosphere = new ProceduralIcosphere(Subdivisions);

        string icoName = $"Icosphere_S{Subdivisions}";

        if (Truncate)
        {
            icosphere = icosphere.Truncated();
            icoName = $"Truncated_{icoName}";
        }

        icosphere.GenerateUVs();
        Mesh mesh = new Mesh
        {
            name = $"{icoName}_Mesh",
            vertices = icosphere.Vertices.Select(v => v.Position * (Size / 2)).ToArray(), //size/2 = radius. Unity primitives are size 1.
            uv = icosphere.Vertices.Select(v => v.UV).ToArray(),
            triangles = ProceduralShape.TriFace.TriFaceListToIndexArray(icosphere.TriFaces),
            normals = icosphere.Vertices.Select(v => v.Normal).ToArray()
        };

        //mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.Optimize();
        GameObject sphereObject = new GameObject($"{icoName}_Object", typeof(MeshFilter), typeof(MeshRenderer));
        sphereObject.GetComponent<MeshFilter>().sharedMesh = mesh;
        sphereObject.GetComponent<MeshRenderer>().material = DefaultMaterial;
        Debug.Log("Created new GameObject");
    }
}
