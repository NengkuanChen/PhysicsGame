using System.Collections.Generic;
using UnityEngine;

namespace Game.Utility
{
    public static class MeshUtility
    {
        public static Mesh CreateQuad(Vector2 gridSize, float vertexSpace, float defaultHeight)
        {
            var gridWidth = gridSize.x;
            var gridLength = gridSize.y;
            var widthVertexCount = Mathf.FloorToInt(gridWidth / vertexSpace) + 1;
            var widthVertexSpace = gridWidth / (widthVertexCount - 1);
            var lengthVertexCount = Mathf.FloorToInt(gridLength / vertexSpace) + 1;
            var lengthVertexSpace = gridLength / (lengthVertexCount - 1);

            var startPosition = new Vector3(-gridWidth * .5f, defaultHeight, -gridLength * .5f);

            var vertices = new List<Vector3>();
            var uvs = new List<Vector2>();
            var triangles = new List<int>();

            for (var y = 0; y < lengthVertexCount; y++)
            {
                for (var x = 0; x < widthVertexCount; x++)
                {
                    var vertexPosition = startPosition + Vector3.right * x * widthVertexSpace +
                                         Vector3.forward * y * lengthVertexSpace;
                    vertices.Add(vertexPosition);
                    var vertexIndex = vertices.Count - 1;
                    if (x != 0 && y != 0)
                    {
                        AddQuad(vertexIndex - widthVertexCount - 1,
                            vertexIndex - 1,
                            vertexIndex,
                            vertexIndex - widthVertexCount,
                            triangles);
                    }

                    uvs.Add(new Vector2((float) x / (widthVertexCount - 1), (float) y / (lengthVertexCount - 1)));
                }
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            return mesh;
        }

        private static void AddQuad(int leftBottom, int leftTop, int rightTop, int rightBottom, List<int> triangles)
        {
            if (triangles == null || leftBottom < 0 || leftTop < 0 || rightTop < 0 || rightBottom < 0)
            {
                return;
            }

            triangles.Add(leftBottom);
            triangles.Add(leftTop);
            triangles.Add(rightBottom);

            triangles.Add(leftTop);
            triangles.Add(rightTop);
            triangles.Add(rightBottom);
        }
    }
}