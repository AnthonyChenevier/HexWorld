using System.Collections.Generic;
using ProceduralShapes;
using UnityEngine;

namespace ProceduralShapes
{
    public class ProceduralCube : ProceduralShape
    {
        public ProceduralCube(int subdivisions)
        {
            Vertices = new List<Vertex>
            {
                new Vertex(-1, -1, -1),
                new Vertex(1, -1, -1),
                new Vertex(1, 1, -1),
                new Vertex(-1, 1, -1),
                new Vertex(-1, 1, 1),
                new Vertex(1, 1, 1),
                new Vertex(1, -1, 1),
                new Vertex(-1, -1, 1),
            };
            TriFaces = new List<TriFace>
            {
                new TriFace(0, 2, 1),
                new TriFace(0, 3, 2),
                new TriFace(2, 3, 4),
                new TriFace(2, 4, 5),
                new TriFace(1, 2, 5),
                new TriFace(1, 5, 6),
                new TriFace(0, 7, 4),
                new TriFace(0, 4, 3),
                new TriFace(5, 4, 7),
                new TriFace(5, 7, 6),
                new TriFace(0, 6, 7),
                new TriFace(0, 1, 6),
            };

            Subdivide(subdivisions);

            //normalize vertices to unit sphere
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertex vertex = Vertices[i];
                vertex = new Vertex(vertex.Position.normalized / 2);
                Vertices[i] = vertex;
            }
        }

        public override void GenerateUVs()
        {

            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertex vertex = Vertices[i];
                vertex.UV = new Vector2(0.5f + Mathf.Atan2(vertex.Position.z, vertex.Position.x) / (2f * Mathf.PI),
                                        0.5f + Mathf.Asin(vertex.Position.y) / Mathf.PI);
                Vertices[i] = vertex;
            }
        }
    }


}