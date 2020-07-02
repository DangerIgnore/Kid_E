using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;



namespace Clap
{
    public class HandInstance : MonoBehaviour
    {

        public MyMesh mesh, meshFEM;
        public int handIndex = -1; //will throw errors is not correct. //TODO ask mickeal how we should  handle this.
        public ClapBehaviour clapBehaviour;

        public int nBones;
        public int nNodes;
        public int nJoints; //including fingertips and wrist position

        //initial values
        public List<Vector3> _vertices;
        public int[] _triangles;
        public List<Vector2> _UV0;
        public List<Vector3> _normals;
        //TArray<FProcMeshTangent> _tangents;  //TODO
        public List<Vector4> _tangents;

        public List<Vector3> _verticesFEM;
        public int[] _trianglesFEM;
        public List<Vector2> _UV0FEM;
        public List<Vector3> _normalsFEM;
        //TArray<FProcMeshTangent> _tangentsFEM;  //TODO


        //Temporary MirroredLeftHand
        public List<Vector3> _verticesMirrored;
        public int[] _trianglesMirrored;


        //duplicated vertices
        public int[][] doubles;
        public int[][] doublesFEM;


        public Vector3 handPos;
        public Quaternion handOrientation;

        // Transformation between CLAP and Unreal reference systems
        //Quaternion transf = new Quaternion(0.5f * Mathf.Sqrt(2), 0f, 0f, 0.5f * Mathf.Sqrt(2));

        //Debugging Lists:
        public List<ClapBone> handBones;
        public List<GameObject> handJoints;

        //Variables to overwrite, not created and destroyed continuosly
        public int nVerticesSkin;
        public int nVerticesFEM;
        public int nFacesFEM;
        public Vector3 coordinates;
        public Vector3 coordinatesUV;
        public Quaternion quatRotation;

        int layer; //layer mask for exporting images to grab only one layer-


        /// <summary>
        /// use this function to give this hand instance a reference to clap behaviour and tell it what hand id it is.
        /// </summary>
        public void Initialize(ClapBehaviour _clapBehaviour, int _handIndex, int _layer)
        {
            clapBehaviour = _clapBehaviour;
            handIndex = _handIndex;
            //layer = _layer;
            //gameObject.layer = _layer;
        }

        private void Start()
        {
            //skin
            nVerticesSkin = clapBehaviour.CLAP.GetNVerticesSkin(handIndex);
            skinVerticesPositions = new double[nVerticesSkin * 3]; //xyz coordinates of skin verticies

            //fem
            nVerticesFEM = clapBehaviour.CLAP.GetNVerticesMesh(handIndex);
            nFacesFEM = clapBehaviour.CLAP.GetNFacesMesh(handIndex);

            
        }

        public void InstantiatePrefabs(ClapBehaviour.Bonetype type, GameObject prefab, Material material, float scale)
        {
            if (handBones == null)
            {
                handBones = new List<ClapBone>();
            }
            string typeS = type.ToString();

            GameObject current;
            GameObject Holder;
            //Create a bone holder object as a child of this transform
            Holder = new GameObject();
            Holder.name = typeS + "_Holder";
            Holder.transform.SetParent(transform);
            //instanciate the bone prefabs
            ClapBone bone;
            for (int i = 0; i < nBones; i++)
            {
                Debug.Log("Instanciating" + typeS + " Prefab: " + i);
                current = Instantiate(prefab) as GameObject;
                current.name = typeS + "_" + i;

                bone = current.GetComponent<ClapBone>();

                current.transform.localScale = 0.01f * Vector3.one * scale;
                handBones.Add(bone);
                current.transform.SetParent(Holder.transform);

                bone.SetMaterial(material);
            }
        }



        double x, y, z;

        double[] skinVerticesPositions;
        int tempCount; int index;
        Vector3 pos = Vector3.zero;
        public void UpdateMeshBackup()
        {
            //get the clap xyz coordinates of each skin vertex
            skinVerticesPositions = clapBehaviour.CLAP.GetVerticesSkin(handIndex, out tempCount);
            index = 0;
            //compute the Unity coordinates of the vertices
            for (int i = 0; i < nVerticesSkin; ++i)
            {

                x = skinVerticesPositions[index++];
                y = skinVerticesPositions[index++];
                z = skinVerticesPositions[index++];

                pos.Set((float)z, (float)y, (float)x);
                _vertices[i] = pos;

                //WAY FASTER to not go through ConvertCalls
                //_vertices[i] = clapBehaviour.ConvertVectorFromToCLAP(skinVerticesPositions[index++], skinVerticesPositions[index++], skinVerticesPositions[index++], clapBehaviour.scale);

            }

            //Update the mesh
            mesh.SetVertices(ref _vertices);
            mesh.Enable(true);
            meshFEM.Enable(false);
        }

        public void ShowSkin(bool on)
        {
            mesh.Enable(on);
        }

        IntPtr skinVerticesPositionsPTR;
        public void UpdateMesh()
        {
            //get the clap xyz coordinates of each skin vertex
            
            float scale = clapBehaviour.scale;
            index = 0;
            //skin pointer
            skinVerticesPositionsPTR = clapBehaviour.CLAP.GetVerticesSkin2(handIndex, out tempCount);
            //Debug.Log("NumVertices in hand : " + nVerticesSkin+ ", hand: "+handIndex);

            _vertices.Clear();
            _vertices.Capacity = tempCount;

            unsafe
            {
                double* dptr = (double*)skinVerticesPositionsPTR.ToPointer();
                //compute the Unity coordinates of the vertices


                for (int i = 0; i < nVerticesSkin; ++i,index+=3)
                {

                    //x = dptr[index];
                    //y = dptr[index+1];
                    //z = dptr[index+2];
                    if (double.IsNaN(dptr[index + 2]) || double.IsNaN(dptr[index + 1]) || double.IsNaN(dptr[index]))
                    {
                        //Debug.LogError("ToInfinityAndBeyond 4: " + dptr[index + 2]+","+ dptr[index + 1]+","+ dptr[index]);
                        //_vertices.Add(Vector3.one*100);
                        _vertices.Populate(tempCount, Vector3.one*0.5f);
                        break;
                    }
                    pos.Set(scale*(float)dptr[index + 2], scale*(float)dptr[index + 1], scale*(float)dptr[index]); //swap x and z     
                    //_vertices[i] = pos /** scale*/;  
                    _vertices.Add(pos /**scale*/); //IMPORTANT: it was around 7ms faster to multiply scale in the line above then an additional operation using Vector3.op_Multiply(pos * scale) 
                    
                    //WAY FASTER to NOT go through ConvertCalls
                    //_vertices[i] = clapBehaviour.ConvertVectorFromToCLAP(skinVerticesPositions[index++], skinVerticesPositions[index++], skinVerticesPositions[index++], clapBehaviour.scale);
                }
            }
            //Debug.Log("_vertices in hand : " + _vertices.Count + ", hand: " + handIndex);

            //Update the mesh
            mesh.SetVertices(ref _vertices);
            ShowSkin(true);
            meshFEM.Enable(false);
        }





        int threadsToUse = 10;
        public int verticiesPerThread;
        public int lostVertices;
        public int threadsStarted = 0, threadsCompleted = 0;

        public float clapBehaviourScale;
        /// <summary>
        /// Update hand mesh through thread paralleization thanks to: https://answers.unity.com/questions/486584/how-to-create-a-parallel-for-in-unity.html
        /// </summary>
        public void UpdateMeshParallelTest(){

            if (threadsStarted == 0)
            {
                clapBehaviourScale = clapBehaviour.scale;
                index = 0;
                //skin pointer
                skinVerticesPositionsPTR = clapBehaviour.CLAP.GetVerticesSkin2(handIndex, out tempCount);
                //Debug.Log("NumVertices in hand : " + nVerticesSkin+ ", hand: "+handIndex);

                //_vertices = new List<Vector3>(tempCount); //causes a null pointer if i set the vertices to size 0    _vertices[100] is null
                //Debug.Log("_vertices.Capacity: " + _vertices.Capacity);

                verticiesPerThread = nVerticesSkin / threadsToUse;
                lostVertices = nVerticesSkin % threadsToUse;

                for (int i = 0; i < threadsToUse; i++)
                {
                    ParameterizedThreadStart pts = new ParameterizedThreadStart(ParallelFor);
                    Thread workerForOneRow = new System.Threading.Thread(pts);
                    workerForOneRow.Start(i);
                    threadsStarted++;
                }
            }
            else if (threadsCompleted >= threadsToUse)
            {
                //Update the mesh
                mesh.SetVertices(ref _vertices);
                mesh.Enable(true);
                meshFEM.Enable(false);

                threadsCompleted = 0;
                threadsStarted = 0;
            }   
        }
        public int tries;
        void ParallelFor(object threadParamsVar)
        {
            int k = (int)threadParamsVar; //thread id
            int from = k * (verticiesPerThread);
            int to = ((k + 1) * verticiesPerThread) - 1;
            if (k == threadsToUse - 1)
            {
                to += lostVertices;
            }
            //Debug.Log("Thread " + k + " - from " + from + " to " + to);
            index = from * 3; //xyz
            Vector3 threadPos = Vector3.zero;

            unsafe
            {
                double* dptr = (double*)skinVerticesPositionsPTR.ToPointer();
                while (dptr == null)
                {
                    dptr = (double*)skinVerticesPositionsPTR.ToPointer();
                    tries++;
                    if(tries > 100) { break; }
                }
                //compute the Unity coordinates of the vertices
                for (int i = from; i <= to; ++i,index +=3)
                {
                    //x = dptr[index];
                    //y = dptr[index+1];
                    //z = dptr[index+2];

                    //threadPos.Set(clapBehaviourScale * (float)dptr[index + 2], clapBehaviourScale * (float)dptr[index + 1], clapBehaviourScale * (float)dptr[index]); //swap x and z     
                    //_vertices[i] = (threadPos /**scale*/); //IMPORTANT: it was around 7ms faster to multiply scale in the line above then an additional operation using Vector3.op_Multiply(pos * scale) 

                    _vertices[i] = new Vector3(clapBehaviourScale * (float)dptr[index + 2], clapBehaviourScale * (float)dptr[index + 1], clapBehaviourScale * (float)dptr[index]);     
                }
            }
            threadsCompleted++;
        }

        public void UpdateFEMMesh()
        {
            //nVerticesFEM = clapBehaviour.CLAP.GetNVerticesMesh(handIndex);
            //nFacesFEM = clapBehaviour.CLAP.GetNFacesMesh(handIndex);

            // Update vertices
            for (int i = 0; i < nVerticesFEM; i++)
            {
                coordinates = clapBehaviour.CLAP.GetVertexMesh(handIndex, i);
                _verticesFEM[i] = (clapBehaviour.ConvertVectorFromToCLAP(coordinates, clapBehaviour.scale));
                //Debug.Log("FemVertice Update: " + i);

               
            }

            // Update mesh
            meshFEM.SetLists(_verticesFEM, _trianglesFEM, _UV0FEM);

            mesh.Enable(false);
            meshFEM.Enable(true);

        }
    }
}