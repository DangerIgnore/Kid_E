using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Clap
{
    //[RequireComponent(typeof(MeshFilter))]
    //[RequireComponent(typeof(MeshRenderer))]
    public class MyMesh : MonoBehaviour
    {
        public MeshFilter filter;
        public MeshRenderer meshRenderer;

        void Awake()
        {
            Setup();

        }

        private void Start()
        {
            //  Debug.Log("I have n verticies in my mesh at start:" + GetMeshVertexCount());

        }
        private void Update()
        {
            // Debug.Log("I have n verticies in my mesh:" + GetMeshVertexCount());
        }

        public void Enable(bool b)
        {
            gameObject.SetActive(b);
        }

        public void SetMesh(Mesh mesh)
        {
            filter.mesh = mesh;
        }
        public Mesh GetMesh()
        {
            return filter.mesh;
        }
        public void SetMaterial(Material mat)
        {
            meshRenderer.material = mat;
        }
        public void SetMaterial(Material mat, int index)
        {
            meshRenderer.materials[index] = mat;
        }
        public Material GetMaterial(int index)
        {
            return meshRenderer.materials[index];
        }

        public int GetMeshVertexCount()
        {
            if (filter == null) { filter = GetComponent<MeshFilter>(); }
            return filter.sharedMesh.vertexCount;
        }
        public int GetNIndexes()
        {
            if (filter == null) { filter = GetComponent<MeshFilter>(); }
            return (int)filter.mesh.GetIndexCount(0);
        }
        public void GetVerticies(List<Vector3> verticies)
        {
            if (filter == null) { filter = GetComponent<MeshFilter>(); }
            filter.sharedMesh.GetVertices(verticies);
        }
        public void GetIndices(List<int> indicies)
        {
            if (filter == null) { filter = GetComponent<MeshFilter>(); }
            filter.mesh.GetIndices(indicies, 0);
        }


        public void SetAll(List<Vector3> vertices, int[] triangles, List<Vector3> normals, List<Vector2> UV0, List<Vector4> tangents)
        {
            filter.mesh.Clear(); //without this, mesh is zombie hand and 4 errors (meshVerticies is too small: https://answers.unity.com/questions/423569/meshvertices-is-too-small.html)
            filter.mesh.SetVertices(vertices);
            filter.mesh.triangles = triangles;
            filter.mesh.SetNormals(normals);
            filter.mesh.SetUVs(0, UV0); //TODO i put that zero, i dont know what it is
                                        //mesh.SetColors(_colors);
            filter.mesh.SetTangents(tangents);

            //filter.mesh.RecalculateNormals();
            //filter.mesh.RecalculateTangents();

            //Debug.Log("Normals:"+filter.mesh.normals.Length);

            /* string s = "";
            for(int i = 0; i < filter.mesh.triangles.Length / 3; i++)
            {
                s += "("+filter.mesh.triangles[i] + ","+filter.mesh.triangles[i+1] + "," + filter.mesh.triangles[i+2] + ")\n";
            }
            Debug.Log("Triangles: " + s);
        */
            //TODO: use mesh or sharedMesh??
        }

        public void SetLists(List<Vector3> vertices, int[] triangles, List<Vector2> UV0)
        {
            filter.mesh.Clear(); //without this, mesh is zombie hand and 4 errors (meshVerticies is too small: https://answers.unity.com/questions/423569/meshvertices-is-too-small.html)
            filter.mesh.SetVertices(vertices);
            filter.mesh.triangles = triangles;
            filter.mesh.SetUVs(0, UV0); //TODO i put that zero, i dont know what it is

            filter.mesh.RecalculateNormals();
            filter.mesh.RecalculateTangents();
            filter.mesh.RecalculateBounds();


            //Debug.Log("Normals:" + filter.mesh.normals.Length);

            /* string s = "";
             for(int i = 0; i < filter.mesh.triangles.Length / 3; i++)
             {
                 s += "("+filter.mesh.triangles[i] + ","+filter.mesh.triangles[i+1] + "," + filter.mesh.triangles[i+2] + ")\n";
             }
             Debug.Log("Triangles: " + s);
             */
        }
        public void SetVertices(ref List<Vector3> vertices)
        {
            filter.mesh.SetVertices(vertices);
            RefreshMesh();
        }

        public void RefreshMesh()
        {
            filter.mesh.RecalculateNormals();
            filter.mesh.RecalculateTangents();
            filter.mesh.RecalculateBounds();
            //Could be optimized if clap returned hand mesh BoundingBox    filter.mesh.bounds = new Bounds(center, size);
        }

        public void SetSimulatePhysics(bool b)
        {
            //gameObject.rigidbody.
        }


        /// <summary>
        /// Copies all the values from one mesh to another.
        /// verticies, triangles, uv, normals, colors, tangents, materials
        /// </summary>
        /// <param name="meshToCopy"></param>
        public void CopyMesh(MyMesh meshToCopy)
        {
            Mesh otherMesh = meshToCopy.filter.mesh;

            filter.mesh.vertices = otherMesh.vertices;
            filter.mesh.triangles = otherMesh.triangles;
            filter.mesh.uv = otherMesh.uv;
            filter.mesh.normals = otherMesh.normals;
            filter.mesh.colors = otherMesh.colors;
            filter.mesh.tangents = otherMesh.tangents;

            //CopyMaterials(meshToCopy);

        }

        void CopyMaterials(MyMesh meshToCopy)
        {
            int numberOfMaterials = meshToCopy.meshRenderer.materials.Length;
            for (int i = 0; i < numberOfMaterials; ++i)
            {
                //set this material to the same as the material the other one has
                //SetMaterial(meshToCopy.GetMaterial(i), i);
                meshRenderer.material.CopyPropertiesFromMaterial(meshToCopy.GetMaterial(i));
            }
        }

        public void SetVisibility(bool visible)
        {
            meshRenderer.enabled = visible;
        }

        /*
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(filter.sharedMesh.bounds.center, filter.sharedMesh.bounds.size);
        }
        */

        //Called by unity when adding the component or calling reset function.
        void Reset()
        {
            Setup();
        }

        public void Setup()
        {
            //this allows user to select mesh filter in inspector, or by default, grabs the one on this gameobject
            if (filter == null)
            {

                //Find it on a child
                filter = GetComponentInChildren<MeshFilter>();

                //if there arent any in child, add a new one
                if (filter == null)
                {
                    filter = gameObject.AddComponent<MeshFilter>();
                    //filter = GetComponent<MeshFilter>();
                }

            }
            if (meshRenderer == null)
            {

                //Find it on a child
                meshRenderer = GetComponentInChildren<MeshRenderer>();

                //if there arent any in child, add a new one
                if (meshRenderer == null)
                {
                    meshRenderer = gameObject.AddComponent<MeshRenderer>();
                    //meshRenderer = GetComponent<MeshRenderer>();
                }
            }
        }

        public Vector3 ComputeTriangleNormal(int triangleId)
        {
            /*
            Vector3 P1 = filter.mesh.normals[filter.mesh.triangles[triangleId * 3]];
            Vector3 P2 = filter.mesh.normals[filter.mesh.triangles[triangleId * 3 + 1]];
            Vector3 P3 = filter.mesh.normals[filter.mesh.triangles[triangleId * 3 + 2]];

            Vector3 faceNormal = ((P1 + P2 + P3) / 3);
            return faceNormal;
            */

            return filter.mesh.normals[triangleId];
        }


    }
    
}