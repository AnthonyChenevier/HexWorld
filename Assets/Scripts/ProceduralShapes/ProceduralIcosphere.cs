// ProceduralIcosphere.cs
// 
// Part of World Testbed - Assembly-CSharp
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2021/02/21 2:11 AM


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralShapes {
    
    public class ProceduralIcosphere: ProceduralShape
    {
        protected static readonly float Phi = (1 + Mathf.Sqrt(5)) * .5f;

        public ProceduralIcosphere()
        {
            Vertices = new List<Vertex>();
            TriFaces = new List<TriFace>();
        }

        public ProceduralIcosphere(int subdivisions)
        {
            Vertices = new List<Vertex>
            {
                new Vertex(-1, Phi, 0),
                new Vertex(1, Phi, 0),
                new Vertex(-1, -Phi, 0),
                new Vertex(1, -Phi, 0),
                new Vertex(0, -1, Phi),
                new Vertex(0, 1, Phi),
                new Vertex(0, -1, -Phi),
                new Vertex(0, 1, -Phi),
                new Vertex(Phi, 0, -1),
                new Vertex(Phi, 0, 1),
                new Vertex(-Phi, 0, -1),
                new Vertex(-Phi, 0, 1)
            };

            TriFaces = new List<TriFace>
            {
                // 5 faces around point 0
                new TriFace(0, 11, 5),
                new TriFace(0, 5, 1),
                new TriFace(0, 1, 7),
                new TriFace(0, 7, 10),
                new TriFace(0, 10, 11),

                // 5 adjacent faces
                new TriFace(1, 5, 9),
                new TriFace(5, 11, 4),
                new TriFace(11, 10, 2),
                new TriFace(10, 7, 6),
                new TriFace(7, 1, 8),

                // 5 faces around point 3
                new TriFace(3, 9, 4),
                new TriFace(3, 4, 2),
                new TriFace(3, 2, 6),
                new TriFace(3, 6, 8),
                new TriFace(3, 8, 9),

                // 5 adjacent faces
                new TriFace(4, 9, 5),
                new TriFace(2, 4, 11),
                new TriFace(6, 2, 10),
                new TriFace(8, 6, 7),
                new TriFace(9, 8, 1)
            };
            //Subdivide Faces
            Subdivide(subdivisions);

            //normalize vertices to unit sphere
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new Vertex(Vertices[i].Position.normalized);
            }

            Debug.Log($"Icosphere (sub:{subdivisions}) generated. Vertex Count = {Vertices.Count}. Triangle count = {TriFaces.Count}");
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

            //detect...
            Dictionary<int, int> visited = new Dictionary<int, int>();
            int vertexIndex = Vertices.Count - 1;
            List<int> wrapped = new List<int>();

            for (int i = 0; i < TriFaces.Count; i++)
            {

                Vector3 texA = Vertices[TriFaces[i].A].UV;
                Vector3 texB = Vertices[TriFaces[i].B].UV;
                Vector3 texC = Vertices[TriFaces[i].C].UV;
                Vector3 texNormal = Vector3.Cross(texB - texA, texC - texA);
                if (texNormal.z > 0)
                    wrapped.Add(i);
            }

            //...and fix
            foreach (int i in wrapped)
            {
                int ta = TriFaces[i].A;
                int tb = TriFaces[i].B;
                int tc = TriFaces[i].C;
                Vertex A = Vertices[ta];
                Vertex B = Vertices[tb];
                Vertex C = Vertices[tc];

                if (A.UV.x < 0.25f)
                {
                    int tempA = ta;
                    if (!visited.TryGetValue(ta, out tempA))
                    {
                        A.UV.x += 1;
                        Vertices.Add(A);
                        vertexIndex++;
                        visited[ta] = vertexIndex;
                        tempA = vertexIndex;
                    }
                    ta = tempA;
                }
                if (B.UV.x < 0.25f)
                {
                    int tempB = tb;
                    if (!visited.TryGetValue(tb, out tempB))
                    {
                        B.UV.x += 1;
                        Vertices.Add(B);
                        vertexIndex++;
                        visited[tb] = tempB = vertexIndex;
                    }
                    tb = tempB;
                }
                if (C.UV.x < 0.25f)
                {
                    int tempC = tc;
                    if (!visited.TryGetValue(tc, out tempC))
                    {
                        C.UV.x += 1;
                        Vertices.Add(C);
                        vertexIndex++;
                        visited[tc] = tempC = vertexIndex;
                    }
                    tc = tempC;
                }
                TriFaces[i] = new TriFace(ta, tb, tc);
            }

            //fix polar UVs
            Vertex north = Vertices.Find(v => v.Position.y == 1);
            Vertex south = Vertices.Find(v => v.Position.y == -1);
            int northIndex = Vertices.IndexOf(north);
            int southIndex = Vertices.IndexOf(south);
            vertexIndex = Vertices.Count - 1;
            for (int i = 0; i < TriFaces.Count; i++)
            {
                if (TriFaces[i].A == northIndex)
                {
                    Vertex B = Vertices[TriFaces[i].B];
                    Vertex C = Vertices[TriFaces[i].C];
                    Vertex newNorth = north;
                    newNorth.UV.x = (B.UV.x + C.UV.x) / 2;
                    vertexIndex++;
                    Vertices.Add(newNorth);
                    TriFaces[i] = new TriFace(vertexIndex, TriFaces[i].B, TriFaces[i].C);

                }
                else if (TriFaces[i].A == southIndex)
                {
                    Vertex B = Vertices[TriFaces[i].B];
                    Vertex C = Vertices[TriFaces[i].C];
                    Vertex newSouth = south;
                    vertexIndex++;
                    newSouth.UV.x = (B.UV.x + C.UV.x) / 2;
                    Vertices.Add(newSouth);
                    TriFaces[i] = new TriFace(vertexIndex, TriFaces[i].B, TriFaces[i].C);
                }
            }
        }
        
        public ProceduralIcosphere Truncated()
        {
            ProceduralIcosphere truncated = new ProceduralIcosphere();

            Dictionary<int, List<int>> triFacesGroupedByVertex = TriFacesGroupedByVertex();

            int originalVertexCount = Vertices.Count;
            //originalVertexCount = 2;

            List<Vertex> dualVertices = TriFaces.Select(t => new Vertex(t.Centroid(Vertices))).ToList();

            Dictionary<int, Vertex> midPointCache = new Dictionary<int, Vertex>();

            int hexCount = 0;
            int pentCount = 0;
            
            //build new n-gons from each vertex in the original shape
            for (int vertexIndex = 0; vertexIndex < originalVertexCount; vertexIndex++)
            {
                List<int> vertexTriFaces = triFacesGroupedByVertex[vertexIndex];
                if (vertexTriFaces.Count == 6)
                    hexCount++;
                else
                    pentCount++;

                Vertex nGonCentreVertex = FindNgonCentre(vertexTriFaces, dualVertices);

                //for each of the TriFaces connected to this vertex
                for (int j = vertexTriFaces.Count - 1; j >= 0; j--)
                {
                    int triIndex = vertexTriFaces[j];

                    TriFace triangle = TriFaces[triIndex];

                    List<int> otherVerts = new List<int> { triangle.A, triangle.B, triangle.C };

                    //sort the triangle- ORIGINAL CODE TRANSCRIBED FROM RUST - DOESN'T WORK
                    //otherVerts.Remove(vertexIndex);
                    //TriFace sortedTriangle = new TriFace(vertexIndex, otherVerts[0], otherVerts[1]);

                    //ROTATE the triangle until the first elelemtn is the right one works
                    while (otherVerts[0] != vertexIndex)
                    {
                        int first = otherVerts[0];
                        otherVerts.RemoveAt(0);
                        otherVerts.Add(first);
                    }

                    TriFace sortedTriangle = new TriFace(otherVerts.ToArray());

                    Vertex newPoint = dualVertices[triIndex];
                    Vertex midABPoint = GetMidPoint(sortedTriangle.A, sortedTriangle.B, vertexTriFaces, triIndex, newPoint, dualVertices, midPointCache);
                    Vertex midACPoint = GetMidPoint(sortedTriangle.A, sortedTriangle.C, vertexTriFaces, triIndex, newPoint, dualVertices, midPointCache);

                    int nGonCentreIndex = truncated.AddVertex(nGonCentreVertex);
                    int newPointIndex = truncated.AddVertex(newPoint);
                    int midABIndex = truncated.AddVertex(midABPoint);
                    int midACIndex = truncated.AddVertex(midACPoint);

                    truncated.TriFaces.Add(new TriFace(newPointIndex, midACIndex, nGonCentreIndex));
                    truncated.TriFaces.Add(new TriFace(midABIndex, newPointIndex, nGonCentreIndex));

                }
            }
            Debug.Log($"Total hexagons: {hexCount}");
            Debug.Log($"Total pentagons: {pentCount}");

            return truncated;
        }

        private Vertex GetMidPoint(int vertexAIndex, 
                                         int vertexBIndex,
                                         List<int> nGonTriFaces, 
                                         int currentTriFaceIndex, 
                                         Vertex newPoint, List<Vertex> dualVertices,
                                         Dictionary<int, Vertex> midPointCache)
        {
            int adjFaceIndex = FindAdjacentFace(vertexAIndex, vertexBIndex, nGonTriFaces, currentTriFaceIndex).GetValueOrDefault();

            Vertex adjPoint = dualVertices[adjFaceIndex];

            int key =  vertexAIndex ^ vertexBIndex ^ (adjFaceIndex << 16);
            if (midPointCache.ContainsKey(key))
                return midPointCache[key];

            Vertex midPoint = new Vertex((newPoint.Position + adjPoint.Position) / 2);
            midPointCache.Add(key, midPoint);
            return midPoint;
        }

        private int? FindAdjacentFace(int vertexAIndex, int vertexBIndex, List<int> nGonTriFaces, int currentTriFaceIndex)
        {
            foreach (int triFaceIndex in nGonTriFaces)
            {
                if (triFaceIndex == currentTriFaceIndex) continue;

                TriFace triangle = TriFaces[triFaceIndex];
                if ((triangle.A == vertexAIndex || triangle.B == vertexAIndex || triangle.C == vertexAIndex) &&
                    (triangle.A == vertexBIndex || triangle.B == vertexBIndex || triangle.C == vertexBIndex))
                {
                    return triFaceIndex;
                }
            }
            return null;
        }

        private Vertex FindNgonCentre(List<int> triIndices, List<Vertex> triCentroids)
        {
            Vector3 centrePoint = Vector3.zero;
            foreach (int triIndex in triIndices)
            {
                centrePoint += triCentroids[triIndex].Position;
            }

            centrePoint /= triIndices.Count;
            return new Vertex(centrePoint);
        }

        public Dictionary<int, List<int>> TriFacesGroupedByVertex()
        {
            Dictionary<int, List<int>> vertToFaces = new Dictionary<int, List<int>>();
            for (int i = 0; i < TriFaces.Count; i++)
            {
                TriFace triangle = TriFaces[i];

                if (vertToFaces.ContainsKey(triangle.A)) vertToFaces[triangle.A].Add(i);
                else vertToFaces.Add(triangle.A, new List<int> { i });

                if (vertToFaces.ContainsKey(triangle.B)) vertToFaces[triangle.B].Add(i);
                else vertToFaces.Add(triangle.B, new List<int> { i });

                if (vertToFaces.ContainsKey(triangle.C)) vertToFaces[triangle.C].Add(i);
                else vertToFaces.Add(triangle.C, new List<int> { i });
            }

            return vertToFaces;
        }
    }
}
