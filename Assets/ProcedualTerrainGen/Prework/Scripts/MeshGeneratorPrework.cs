using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualTerrainGen.Prework
{
    [RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
    public class MeshGeneratorPrework : MonoBehaviour
    {
        [SerializeField] private Material _mat;

        [SerializeField] private int xSize, zSize;
        
        private void Start()
        {
            Mesh mesh = new Mesh();
            
            List<Vector3> vertices = new List<Vector3>();
            for (var i = 0; i <= zSize; i++)
            {
                for (var j = 0; j <= xSize; j++)
                {
                    vertices.Add(new Vector3(i- (xSize/2f), 0, j-(zSize/2f)));
                }
            }
            mesh.SetVertices(vertices);

//            int[] tris = new int[xSize * 6 * zSize];
//            for (int i = 0; i <= zSize; i++)
//            {
//                for (int j = 0; j < xSize; j++)
//                {
//                    var offset = (i * (xSize + 1) + j) * 3 * 2;
//                    int CalcVerticesIndex(int _i, int _j) => _i * (xSize + 1) + _j;
//
//                    tris[offset + 0] = CalcVerticesIndex(i, j);
//                    tris[offset + 1] = CalcVerticesIndex(i, j + 1);
//                    tris[offset + 2] = CalcVerticesIndex(i + 1, j);
//                    
//                    tris[offset + 3] = CalcVerticesIndex(i, j + 1);
//                    tris[offset + 4] = CalcVerticesIndex(i + 1, j + 1);
//                    tris[offset + 5] = CalcVerticesIndex(i + 1, j);
//
//                }
//            }

//            int[] tris = new[]
//            {
//                0, 1, 6,
//                7,6,1
//            };
//            mesh.triangles = tris;

            List<int> triangles = new List<int>();
            for(int i=0; i<zSize; i++)
            for (int j = 0; j < xSize; j++)
            {
                Func<int, int, int> calcVertexIndex = (_i, _j) => _i * (xSize + 1) + _j;
                
                triangles.Add(calcVertexIndex(i, j));
                triangles.Add(calcVertexIndex(i, j + 1));
                triangles.Add(calcVertexIndex(i + 1, j));

                triangles.Add(calcVertexIndex(i, j + 1));
                triangles.Add((calcVertexIndex(i + 1, j + 1)));
                triangles.Add(calcVertexIndex(i+1,j));
            }
            mesh.SetTriangles(triangles, 0);
            
            mesh.RecalculateNormals();
            MeshFilter filter = GetComponent<MeshFilter>();
            
            filter.sharedMesh = mesh;

            MeshRenderer renderer = GetComponent<MeshRenderer>();
            renderer.material = _mat;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach(var v in GetComponent<MeshFilter>().sharedMesh.vertices)
            {
                Gizmos.DrawSphere(v, 0.1f);
            }
        }
    }
}