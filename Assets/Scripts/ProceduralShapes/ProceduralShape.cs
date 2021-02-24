// ProceduralShape.cs
// 
// Part of World Testbed - Assembly-CSharp
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2021/02/20 2:39 PM


using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralShapes
{
    public class ProceduralShape
    {
        public struct Vertex
        {
            public Vector2 UV;
            public readonly Vector3 Position;
            public readonly Vector3 Normal;
            
            public Vertex(float x, float y, float z) : this(new Vector3(x, y, z)) { }

            public Vertex(Vector3 vertexPosition)
            {
                Position = vertexPosition;
                Normal = vertexPosition.normalized; //smooth shading

                UV = Vector2.zero; //unknown UVs
            }

            public override string ToString()
            {
                return $"V:({Position})({Normal})({UV})";
            }
        }

        public struct TriFace
        {
            private readonly int[] _indices;

            public int A => _indices[0];
            public int B => _indices[1];
            public int C => _indices[2];

            public TriFace(params int[] indices) { _indices = indices; }

            public static int[] TriFaceListToIndexArray(List<TriFace> triangles)
            {
                return triangles.SelectMany(t => t._indices).ToArray();
            }

            public Vector3 Centroid(List<Vertex> vertices)
            {
                return (vertices[A].Position + vertices[B].Position + vertices[C].Position) / 3.0f;
            }

            public override string ToString()
            {
                return $"TriFace({A},{B},{C})";
            }
        }

        public List<TriFace> TriFaces = new List<TriFace>();
        public List<Vertex> Vertices = new List<Vertex>();

        protected readonly Dictionary<Vertex, int> NewVertexCache = new Dictionary<Vertex, int>();

        public void Subdivide(int subdivisions)
        {
            NewVertexCache.Clear();
            for (int i = 0; i < subdivisions; i++)
            {
                List<TriFace> newTris = new List<TriFace>();
                foreach (TriFace triangle in TriFaces)
                {
                    int a = AddMidPoint(triangle.A, triangle.B);
                    int b = AddMidPoint(triangle.B, triangle.C);
                    int c = AddMidPoint(triangle.C, triangle.A);

                    newTris.Add(new TriFace(triangle.A, a, c));
                    newTris.Add(new TriFace(triangle.B, b, a));
                    newTris.Add(new TriFace(triangle.C, c, b));
                    newTris.Add(new TriFace(a, b, c));
                }
                TriFaces.Clear();
                TriFaces = newTris;
            }
        }

        private int AddMidPoint(int vertexIndexA, int vertexIndexB)
        {
            return AddMidPoint(Vertices[vertexIndexA], Vertices[vertexIndexB]);
        }

        private int AddMidPoint(Vertex a, Vertex b)
        {
            return AddVertex(new Vertex((a.Position + b.Position) / 2));
        }


        public int AddVertex(Vertex vertex) 
        {
            int vertIndex;

            //just return the index from the cache if the vertex already exists
            if (NewVertexCache.TryGetValue(vertex, out vertIndex))
                return vertIndex;

            //we don't have the vertex in the cache, so add it to both
            //the Vertices and NewVertexCache lists then return the new index
            Vertices.Add(vertex);
            vertIndex = Vertices.Count - 1;
            NewVertexCache.Add(vertex, vertIndex);

            return vertIndex;
        }

        public virtual void GenerateUVs() { throw new System.NotImplementedException(); }
    }
}
