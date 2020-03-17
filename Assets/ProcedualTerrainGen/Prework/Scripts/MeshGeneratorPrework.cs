using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualTerrainGen.Prework
{
    [RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
    public class MeshGeneratorPrework : MonoBehaviour
    {
        [SerializeField] private bool isDrawGizmo=false;
        
        [SerializeField] private Material _mat;

        [SerializeField] private int xSize, zSize;

        [SerializeField] private float _geometryScale;
        [SerializeField] private float _xOffset, _zOffset;
        
        
        
        private void Start()
        {
            UpdateMesh();
        }

        private void OnValidate()
        {
            UpdateMesh();
        }

        void UpdateMesh()
        {
            var mesh = new Mesh();
            
            mesh.SetVertices(CreateVertices(_geometryScale, _xOffset, _zOffset));
            mesh.SetTriangles(CreateTriangles(), 0);
            
            mesh.RecalculateNormals();
            var filter = GetComponent<MeshFilter>();
            
            filter.sharedMesh = mesh;

            var renderer = GetComponent<MeshRenderer>();
            renderer.material = _mat;
        }

        List<Vector3> CreateVertices(float geometryScale, float xOffset, float zOffset)
        {
            var vertices = new List<Vector3>();
            for (var i = 0; i <= zSize; i++)
            {
                for (var j = 0; j <= xSize; j++)
                {

                    float Noise(float _xvalue, float _zvalue)
                    {
                        return Mathf.PerlinNoise(_xvalue, _zvalue) * 2.0f - 1.0f;
                    }
                    
                    var _x = (i - (zSize / 2f)) * geometryScale;
                    var _z = (j - (xSize / 2f)) * geometryScale;
                    

                    var _y =
                            (
                            Noise(_x / 100f + xOffset, _z / 100f + zOffset)*50
                            + Noise(_x/5f+xOffset, _z/5f+zOffset)*3
                            + Noise(_x+xOffset, _z+zOffset)
                            )
                        ;


                    vertices.Add(new Vector3(_x, _y, _z)*3f);
                }
            }

            return vertices;
        }

        List<int> CreateTriangles()
        {
            var triangles = new List<int>();
            for(var i=0; i<zSize; i++)
            for (var j = 0; j < xSize; j++)
            {
                Func<int, int, int> calcVertexIndex = (_i, _j) => _i * (xSize + 1) + _j;
                
                triangles.Add(calcVertexIndex(i, j));
                triangles.Add(calcVertexIndex(i, j + 1));
                triangles.Add(calcVertexIndex(i + 1, j));

                triangles.Add(calcVertexIndex(i, j + 1));
                triangles.Add((calcVertexIndex(i + 1, j + 1)));
                triangles.Add(calcVertexIndex(i+1,j));
            }

            return triangles;
        }

        private void OnDrawGizmos()
        {
            if (!isDrawGizmo) return;
            
            Gizmos.color = Color.yellow;
            foreach(var v in GetComponent<MeshFilter>().sharedMesh.vertices)
            {
                Gizmos.DrawSphere(v, 0.1f);
            }
        }
    }
}