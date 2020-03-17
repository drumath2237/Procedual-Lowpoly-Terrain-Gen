using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcedualTerrainGen.Prework
{
    enum Biome
    {
        Sand,
        Grass,
        Rock,
        Snow
    }

    internal struct PolygonWithBiome
    {
        public List<int> vertices;
        public Biome biome;
    }
    
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

            var vertices = CreateVertices(_geometryScale, _xOffset, _zOffset);
            mesh.SetVertices(vertices);

            mesh.subMeshCount = 4;

            var sandMesh = new List<int>();
            var grassMesh = new List<int>();
            var rockMesh = new List<int>();
            var snowMesh = new List<int>();
            
            for(var i=0;i<zSize;i++)
            for (var j = 0; j < xSize; j++)
            {
                var poly = CreateBiomePolygonSide1(i, j, vertices);
                switch (poly.biome)
                {
                    case Biome.Sand:
                        sandMesh.AddRange(poly.vertices);
                        break;
                    case Biome.Grass:
                        grassMesh.AddRange(poly.vertices);
                        break;
                    case Biome.Rock:
                        rockMesh.AddRange(poly.vertices);
                        break;
                    case Biome.Snow:
                        snowMesh.AddRange(poly.vertices);
                        break;
                    default:
                        break;
                }
                
                poly = CreateBiomePolygonSide2(i, j, vertices);
                switch (poly.biome)
                {
                    case Biome.Sand:
                        sandMesh.AddRange(poly.vertices);
                        break;
                    case Biome.Grass:
                        grassMesh.AddRange(poly.vertices);
                        break;
                    case Biome.Rock:
                        rockMesh.AddRange(poly.vertices);
                        break;
                    case Biome.Snow:
                        snowMesh.AddRange(poly.vertices);
                        break;
                    default:
                        break;
                }
            }

            mesh.SetTriangles(sandMesh, 0);
            mesh.SetTriangles(grassMesh, 1);
            mesh.SetTriangles(rockMesh, 2);
            mesh.SetTriangles(snowMesh, 3);



            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            var filter = GetComponent<MeshFilter>();
            
            filter.sharedMesh = mesh;

            var renderer = GetComponent<MeshRenderer>();
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
                            Noise(_x / 100f + xOffset, _z / 100f + zOffset)*47
                            + Noise(_x/20f+xOffset, _z/20f+zOffset)
                            + Noise(_x+xOffset, _z+zOffset)*5
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

        Biome JudgeBiome(float height)
        {
            if (height < -60f)
                return Biome.Sand;
            else if (height < 20)
                return Biome.Grass;
            else if (height < 70)
                return Biome.Rock;
            else
                return Biome.Snow;
        }

        PolygonWithBiome CreateBiomePolygonSide1(int _vertex_index_i, int _vertex_index_j, List<Vector3> vertices)
        {
            PolygonWithBiome polygon = new PolygonWithBiome();
            polygon.vertices = new List<int>();
            
            Func<int, int, int> calcVertexIndex = (_i, _j) => _i * (xSize + 1) + _j;
                
            polygon.vertices.Add(calcVertexIndex(_vertex_index_i, _vertex_index_j));
            polygon.vertices.Add(calcVertexIndex(_vertex_index_i, _vertex_index_j + 1));
            polygon.vertices.Add(calcVertexIndex(_vertex_index_i + 1, _vertex_index_j));

            var averageHeight = ((vertices[polygon.vertices[0]] + vertices[polygon.vertices[1]] +
                                    vertices[polygon.vertices[2]]) / 3.0f).y;
            polygon.biome = JudgeBiome(averageHeight);

            return polygon;
        }
        PolygonWithBiome CreateBiomePolygonSide2(int _vertex_index_i, int _vertex_index_j, List<Vector3> vertices)
        {
            PolygonWithBiome polygon = new PolygonWithBiome();
            polygon.vertices = new List<int>();
            
            Func<int, int, int> calcVertexIndex = (_i, _j) => _i * (xSize + 1) + _j;
                
            polygon.vertices.Add(calcVertexIndex(_vertex_index_i, _vertex_index_j + 1));
            polygon.vertices.Add((calcVertexIndex(_vertex_index_i + 1, _vertex_index_j + 1)));
            polygon.vertices.Add(calcVertexIndex(_vertex_index_i+1,_vertex_index_j));

            var averageHeight = ((vertices[polygon.vertices[0]] + vertices[polygon.vertices[1]] +
                                    vertices[polygon.vertices[2]]) / 3.0f).y;
            polygon.biome = JudgeBiome(averageHeight);

            return polygon;
        }

        private void OnDrawGizmos()
        {
            if (!isDrawGizmo) return;
            
            Gizmos.color = Color.yellow;
            foreach(var v in GetComponent<MeshFilter>().sharedMesh.vertices)
            {
                Gizmos.DrawSphere(v, 1f);
            }
        }
    }
}