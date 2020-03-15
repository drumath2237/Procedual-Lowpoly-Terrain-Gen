using System;
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

            Vector3[] vecs = new Vector3[(xSize + 1) * (zSize + 1)];
            for(var i=0; i<=xSize; i++)
            {
                for (var j = 0; j <= zSize; j++)
                {
                    Vector3 vec = new Vector3(i- (xSize/2f), 0, j-(zSize/2f));
                    vecs[i * (xSize + 1) + j] = vec;
                }
                
            }
            mesh.vertices = vecs;


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