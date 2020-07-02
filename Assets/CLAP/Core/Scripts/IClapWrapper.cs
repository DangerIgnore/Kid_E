using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

/// <summary>
/// https://answers.unity.com/questions/1200157/nonstatic-extern-functions-from-dll-plugin-import.html
/// https://answers.unity.com/questions/49304/creating-an-instance-of-a-c-plugin.html
/// https://stackoverflow.com/questions/14816970/calling-an-unmanaged-library-function-that-takes-a-reference-to-a-pointer
/// </summary>
namespace Clap
{
    public class IClapWrapper
    {

        //C# stuff
        private bool successFullGetClap;
        IntPtr nativeLibraryPtr;

        /// <summary>
        /// Call this function to protect Unity from crashing if the DLL isnt loaded or GetClap Failed.  Consider a better alternative
        /// If true->  BAD
        /// if False-> Good =)
        /// </summary>
        /// <returns></returns>
        public bool EmergencyExit()
        {
            if (!successFullGetClap)
            {
                Debug.LogError("CLAPDLL.dll wasnt loaded or failed in GetCLAP, didnt compute this method");
                return true;
            }
            else
            {
                return false;
            }
        }

        ClapBehaviour m_clapBehaviour;

        public IClapWrapper(ClapBehaviour behaviour)
        {/*
            if (nativeLibraryPtr != IntPtr.Zero)
            {
               //return;
            }
            else
            {

               nativeLibraryPtr = Native.LoadLibrary("CLAPDLL.dll");
               if (nativeLibraryPtr == IntPtr.Zero)
               {
                  Debug.LogError("Failed to load native library");
               }
            }
            */

            m_clapBehaviour = behaviour;

            successFullGetClap = CPlusWrapper.GetCLAPCSharp();
            Debug.Log("GET CLAP:" + successFullGetClap);


            //Reset? 
            //CPlusWrapper.Reset();



            /*
                if (m_CPPClassID.Equals(IntPtr.Zero)){
                   Debug.LogError("Couldnt load CLAPDLL.dll or failed in GetCLAP");
                    return;
                }
                else{
                   Debug.Log("GetCLAP id:" + m_CPPClassID);
                }

            */



        }



        public void DestroyCLAP()
        {
            //DestroyCLAP(ref m_CPPClassID);
            new WaitForSeconds(1.0f);
            if (CPlusWrapper.DestroyCLAPCSharp())
            {
                Debug.Log("Successfully destroyed CLAP object");
            }
            else
            {
                Debug.Log("Could not destroy CLAP object");
            }

        }
        public bool TerminateClap()
        {
            if (CPlusWrapper.TerminateCLAPCSharp())
            {
                Debug.Log("Successfully terminated CLAP object");
                return true;
            }
            else
            {
                Debug.Log("Could not terminate CLAP object");
            }
            return false;
        }

        /*
        public void MyFunction(int aParam1)
        {
            SomeClass_MyFunction(m_CPPClassID, aParam1);
        }

        */

        #region C# Functions
        /************************************************************
         *                      C# Functions
         * **********************************************************/
        double x, y, z, w, rw, rx, ry, rz, ratiod;

        public void SetTrackingMethod(int trackingMethod)
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.SetTrackingMethod(trackingMethod);
        }
        public void SetSimulationMode(int simulationMode)
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.SetSimulationMode(simulationMode);
        }
        public void SetDataSetExportPath(string datasetExportPath)
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.SetDataSetExportPath(datasetExportPath.ToCharArray()/*System.Text.Encoding.UTF8.GetBytes(datasetExportPath)*/);
        }
        /// <summary>
        /// Creates and adds n simulation hand instances to the hand collection
        /// </summary>
        /// <param name="n"></param>
        public void CreateNHands(int n)
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.CreateNHands(n);
        }

        /// <summary>
        /// Initialize the hand simulation and tracker
        /// </summary>
        /// <param name="type"></param>
        public bool Initialize(int type, string resourcePath, bool debugRedirect, int numHands)
        {
            if (EmergencyExit()) { return false; }

            //Debug.Log("Path: " + resourcePath);
            return CPlusWrapper.Initialize(type, resourcePath.ToCharArray() /*System.Text.Encoding.UTF8.GetBytes(resourcePath)*/, debugRedirect, numHands);
        }
        public void EnableCoupling()
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.EnableCoupling();
        }
        /// <summary>
        /// Adds an infinite contact plane to CLAP's scene
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="pz"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        public void AddContactPlane(double px, double py, double pz, double nx, double ny, double nz)
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.AddContactPlane(px, py, pz, nx, ny, nz);
        }
        /// <summary>
        ///Set the location of the hand in world coordinates
        /// </summary>
        /// <param name="handPos"></param>
        public void SetHandPosition(int handIndex, Vector3 handPos)
        {
            if (EmergencyExit()) { return; }
            //Vector3d handPosDoubles = handPos.ToDoubleVector();
            //SetHandPosition(m_CPPClassID, handPosDoubles.x, handPosDoubles.y, handPosDoubles.z);
            CPlusWrapper.SetHandPosition(handIndex, (double)handPos.x, (double)handPos.y, (double)handPos.z);

        }
        /// <summary>
        /// Set the > of the hand in world coordinates
        /// </summary>
        /// <param name="handOrientation"></param>
        public void SetHandOrientation(int handIndex, Quaternion handOrientation)
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.SetHandOrientation(handIndex, (double)handOrientation.w, (double)handOrientation.x, (double)handOrientation.y, (double)handOrientation.z);
        }
        /// <summary>
        ///Get the location of the hand in world coordinates
        /// </summary>
        /// <param name="handPos"></param>
        public Vector3 GetHandPosition(int handIndex)
        {
            if (EmergencyExit()) { return Vector3.zero; }
            CPlusWrapper.GetHandPosition(handIndex, out x, out y, out z);
            //return new Vector3d(x, y, z).ToFloatVector();
            return Vector3.zero.Set(x, y, z);

        }
        /// <summary>
        /// Get the orientation of the hand in world coordinates
        /// </summary>
        /// <param name="handOrientation"></param>
        public Quaternion GetHandOrientation(int handIndex)
        {
            if (EmergencyExit()) { return Quaternion.identity; }
            CPlusWrapper.GetHandOrientation(handIndex, out w, out x, out y, out z);
            //return new Quaternion((float)x, (float)y, (float)z, (float)w);
            return Quaternion.identity.Set(x, y, z, w);
        }



        /// <summary>
        /// Starts the Simulation
        /// </summary>
        public void StartSimulation()
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.StartSimulation();
        }
        /// <summary>
        /// Starts the Simulation
        /// </summary>
        public void StopSimulation()
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.StopSimulation();
        }

        /// <summary>
        /// Returns the number of vertices in the skin
        /// </summary>
        /// <returns></returns>
        public int GetNVerticesSkin(int handIndex)
        {
            if (EmergencyExit()) { return 0; }
            return CPlusWrapper.GetNVerticesSkin(handIndex);
        }
        public int GetTotalNVerticesSkin()
        {
            if (EmergencyExit()) { return 0; }
            return CPlusWrapper.GetTotalNVerticesSkin();
        }


        public Vector3 GetVertexSkin(int handIndex, int vertex)
        {
            if (EmergencyExit()) { return Vector3.zero; }
            CPlusWrapper.GetVertexSkin(handIndex, vertex, out x, out y, out z);
            //return new Vector3d(x, y, z).ToFloatVector();
            return Vector3.zero.Set(x, y, z);
        }
        double[] returnedResult;
        public double[] GetVerticesSkin(int handIndex, out int count)
        {
            if (EmergencyExit()) {count = 0; return new double[0]; }

            //Call and return the pointer
            IntPtr returnedPtr = CPlusWrapper.GetVerticesSkin(handIndex, out count);

            /* //Create new Variable to Store the result
             byte[] returnedResult = new byte[count];

             //Copy from result pointer to the C# variable
             Marshal.Copy(returnedPtr, returnedResult, 0, count);


             //Convert byte to double to Float
 //            double[] doubleValues = new double[count/*returnedResult.Length / 8*/    //];
                                                                                        //          Buffer.BlockCopy(returnedResult, 0, doubleValues, 0, doubleValues.Length /** 8*/);

            //            float[] floats = new float[count];
            //           Buffer.BlockCopy(doubleValues, 0, floats, 0, count);
            //           Debug.LogError("TESTING!" + floats.Length+ " - "+count);

            if (returnedResult == null)
            {
                returnedResult = new double[count * 3];
            }
            //Copy from result pointer to the C# variable
            Marshal.Copy(returnedPtr, returnedResult, 0, count*3);

            return returnedResult;
        }
        public IntPtr GetVerticesSkin2(int handIndex, out int count)
        {
            if (EmergencyExit()) { count = 0; return new IntPtr(); }

            //Call and return the pointer
            IntPtr returnedPtr = CPlusWrapper.GetVerticesSkin(handIndex, out count);
            return returnedPtr;
        }






        /// <summary>
        /// Get coordinates of vertex in mesh
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public Vector3 GetVertexMesh(int handIndex, int vertex)
        {
            if (EmergencyExit()) { return Vector3.zero; }
            CPlusWrapper.GetVertexMesh(handIndex, vertex, out x, out y, out z);
            //return new Vector3d(x, y, z).ToFloatVector();
            return Vector3.zero.Set(x, y, z);

        }

        /// <summary>
        /// Gets the current position of the object (for debugging)
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public Vector3 GetRigidBodyPosition(int id)
        {
            if (EmergencyExit()) { return Vector3.zero; }
            CPlusWrapper.GetRigidBodyPosition(id, out x, out y, out z);
            //return new Vector3d(x, y, z).ToFloatVector();
            return Vector3.zero.Set(x, y, z);


        }
        /// <summary>
        /// Gets the current orientation of the object (for debugging)
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public Quaternion GetRigidBodyOrientation(int id)
        {
            if (EmergencyExit()) { return Quaternion.identity; }
            CPlusWrapper.GetRigidBodyOrientation(id, out w, out x, out y, out z);
            //return new Quaternion((float)x, (float)y, (float)z, (float)w);
            return Quaternion.identity.Set(x, y, z, w);
        }





        /// <summary>
        /// Get texture coordinates of vertex in skin
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public Vector2 GetUVSkin(int handIndex, int vertex)
        {
            if (EmergencyExit()) { return Vector3.zero; }
            CPlusWrapper.GetUVSkin(handIndex, vertex, out x, out y);
            //return new Vector2d(u, v).ToFloatVector();
            return Vector2.zero.Set(x, y);
        }

        /// <summary>
        ///Returns the number of vertices/faces in the skin
        /// </summary>
        /// <returns></returns>
        public int GetNFacesSkin(int handIndex)
        {
            if (EmergencyExit()) { return 0; }
            return CPlusWrapper.GetNFacesSkin(handIndex);
        }
        public int GetTotalNFacesSkin()
        {
            if (EmergencyExit()) { return 0; }
            return CPlusWrapper.GetTotalNFacesSkin();
        }


        /// <summary>
        /// Get indices of a face in skin
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public int[] GetFaceSkin(int handIndex, int face)
        {
            if (EmergencyExit()) { return new int[] { 0, 0, 0 }; }

            int a, b, c;
            CPlusWrapper.GetFaceSkin(handIndex, face, out a, out b, out c);
            return new int[] { a, b, c };

        }
        /// <summary>
        /// Get indices of a face in mesh
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public int[] GetFaceMesh(int handIndex, int face)
        {
            if (EmergencyExit()) { return new int[] { 0, 0, 0 }; }

            int a, b, c;
            CPlusWrapper.GetFaceMesh(handIndex, face, out a, out b, out c);
            return new int[] { a, b, c };

        }


        /*Enable/Disable tracking*/
        public void EnableTracking()
        {
            CPlusWrapper.EnableTracking();
        }
        public void DisableTracking()
        {
            CPlusWrapper.DisableTracking();
        }

        public enum CLAPTrackerOption
        {
            NO_HMD = 0,
            CLAP_TRACKER_HMD_MODE = 1, /*Set this option if the tracker is used in HMD mode*/
        };
        public void SetTrackingOption(int handIndex, CLAPTrackerOption optionName, bool option)
        {
            CPlusWrapper.SetTrackingOption(handIndex, (int)optionName, option);
        }



        /*Enable/Disable Simulation*/
        public void EnableSimulation()
        {
            CPlusWrapper.EnableSimulation();
        }
        public void DisableSimulation()
        {
            CPlusWrapper.DisableSimulation();
        }

        /*Reset simulation*/
        public void Reset()
        {
            CPlusWrapper.Reset();
        }
        /// <summary>
        /// Locks the scene in order to prevent multiple threads accessing scene data
        /// </summary>
        public void LockScene()
        {
            CPlusWrapper.LockScene();
        }
        public void UnlockScene()
        {
            CPlusWrapper.UnlockScene();
        }
        /// <summary>
        /// Communication with host virtual environment. Sets mass and inertia tensor, returns CLAP id
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="i11"></param>
        /// <param name="i22"></param>
        /// <param name="i33"></param>
        /// <param name="i21"></param>
        /// <param name="i31"></param>
        /// <param name="i32"></param>
        /// <param name="scale"></param>
        /// <param name="type"></param>
        /// <param name="isStationary"></param>
        /// <returns></returns>
        public int AddRigidBody(double mass, double i11, double i22, double i33, double i21, double i31, double i32, double scale, int type, bool isStationary)
        {
            return CPlusWrapper.AddRigidBody(mass, i11, i22, i33, i21, i31, i32, scale, type, isStationary);
        }


        /// <summary>
        /// Sets the number of vertices and faces for a particular rigid body. Used for transfering the geometry. The id is obtained by calling addRigidBody first 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nVertices"></param>
        /// <param name="nFaces"></param>
        public void SetRigidBodyNVerticesAndFaces(int id, int nVertices, int nFaces)
        {
            CPlusWrapper.SetRigidBodyNVerticesAndFaces(id, nVertices, nFaces);
        }

        /// <summary>
        /// Sets a vertex of a rigid body in case it is not a predefined object
        /// </summary>
        /// <param name="bodyId"></param>
        /// <param name="vertexId"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void SetRigidBodyVertex(int bodyId, int vertexId, double x, double y, double z)
        {
            CPlusWrapper.SetRigidBodyVertex(bodyId, vertexId, x, y, z);
        }

        /// <summary>
        /// Sets the indices of a face for a rigid body in case it is not a predefined object
        /// </summary>
        /// <param name="bodyId"></param>
        /// <param name="faceId"></param>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public void SetRigidBodyFace(int bodyId, int faceId, int v0, int v1, int v2)
        {
            CPlusWrapper.SetRigidBodyFace(bodyId, faceId, v0, v1, v2);
        }

        /// <summary>
        /// Initializes the rigid body given the provided geometry. Initializes the SDFs 
        /// </summary>
        /// <param name="id"></param>
        public void SetupRigidBody(int id)
        {
            CPlusWrapper.SetupRigidBody(id);
        }


        /*******
         * Update Functions
         * *******/
        public int StepsFinished()
        {
            return CPlusWrapper.StepsFinished();
        }
        /// <summary>
        /// Updates the mesh used for rendering
        /// </summary>
        /// <returns></returns>
        public void UpdateMesh()
        {
            CPlusWrapper.UpdateMesh();
        }

        /// <summary>
        /// Sets the current position of the object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="initial"></param>
        public void SetRigidBodyPosition(int id, Vector3 pos, bool initial)
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.SetRigidBodyPosition(id, (double)pos.x, (double)pos.y, (double)pos.z, initial);
        }
        /// <summary>
        /// Sets the current orientation of the object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rot"></param>
        /// <param name="initial"></param>
        public void SetRigidBodyOrientation(int id, Quaternion rot, bool initial)
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.SetRigidBodyOrientation(id, (double)rot.w, (double)rot.x, (double)rot.y, (double)rot.z, initial);

        }

        public void SetRigidBodyCouplingStiffness(int id, InteractionObject.CouplingSpringConfiguration constants)
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.SetRigidBodyCouplingStiffness(id, constants.linearStiffness, constants.angularStiffness, constants.linearDamping , constants.angularDamping);
        }

        /// <summary>
        /// Queries if coupling is enabled
        /// </summary>
        public bool CouplingEnabled() { return CPlusWrapper.CouplingEnabled(); }

        /// <summary>
        /// Queries if an object is in contact with the hand
        /// </summary>
        public bool ObjectIsInContact(int id) { return CPlusWrapper.ObjectIsInContact(id); }


        /// <summary>
        /// Gets the current force applied on the object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vector3 GetRigidBodyForce(int id)
        {
            if (EmergencyExit()) { return Vector3.zero; }
            CPlusWrapper.GetRigidBodyForce(id, out x, out y, out z);
            //return new Vector3d(x, y, z).ToFloatVector();
            return Vector3.zero.Set(x, y, z);
        }
        /// <summary>
        /// Gets the current torque applied on the object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vector3 GetRigidBodyTorque(int id)
        {
            if (EmergencyExit()) { return Vector3.zero; }
            CPlusWrapper.GetRigidBodyTorque(id, out x, out y, out z);
            //return new Vector3d(x, y, z).ToFloatVector();
            return Vector3.zero.Set(x, y, z);
        }
        /// <summary>
        /// Gets the current linear velocity of the object (for debugging)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vector3 GetRigidBodyLinearVelocity(int id)
        {
            if (EmergencyExit()) { return Vector3.zero; }
            CPlusWrapper.GetRigidBodyLinearVelocity(id, out x, out y, out z);
            //return new Vector3d(x, y, z).ToFloatVector();
            return Vector3.zero.Set(x, y, z);
        }

        /// <summary>
        /// Gets the current angular velocity of the object (for debugging)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vector3 GetRigidBodyAngularVelocity(int id)
        {
            if (EmergencyExit()) { return Vector3.zero; }
            CPlusWrapper.GetRigidBodyAngularVelocity(id, out x, out y, out z);
            //return new Vector3d(x, y, z).ToFloatVector();
            return Vector3.zero.Set(x, y, z);
        }
        /// <summary>
        /// Gets the current position of the proxy object (for debugging)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vector3 GetRigidBodyProxyPosition(int id)
        {
            if (EmergencyExit()) { return Vector3.zero; }
            CPlusWrapper.GetRigidBodyProxyPosition(id, out x, out y, out z);
            //return new Vector3d(x, y, z).ToFloatVector();
            return Vector3.zero.Set(x, y, z);
        }


        /// <summary>
        /// Gets the current orientation of the proxy object (for debugging)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public Quaternion GetRigidBodyProxyOrientation(int id, bool proxy)
        {
            if (EmergencyExit()) { return Quaternion.identity; }

            CPlusWrapper.GetRigidBodyProxyOrientation(id, out w, out x, out y, out z, proxy);
            //return new Quaternion((float)x, (float)y, (float)z, (float)w);
            return Quaternion.identity.Set(x, y, z, w);
        }


        /// <summary>
        /// Set the stiffness and mu friction coeficients for the given RigidBody
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stiffness"></param>
        /// <param name="mu"></param>
        public void UpdateFrictionStiffnessAndCoefficient(int id, double stiffness, double mu)
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.UpdateFrictionStiffnessAndCoefficient(id, stiffness, mu);
        }

        /// <summary>
        /// Sets the Coupling Parameters for the given RigedBody
        /// </summary>
        /// <param name="id"></param>
        /// <param name="linearStiffness"></param>
        /// <param name="angularStiffness"></param>
        /// <param name="linearDamping"></param>
        /// <param name="angularDamping"></param>
        public void SetCouplingParams(int id, double linearStiffness, double angularStiffness, double linearDamping, double angularDamping)
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.SetCouplingParams(id, linearStiffness, angularStiffness, linearDamping, angularDamping);
        }

        /// <summary>
        /// Updates one frame in the clap simulation
        /// </summary>
        public void LoadNextFrame()
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.LoadNextFrame();
        }
        /// <summary>
        /// Returns true when the one frame has been completly process
        /// </summary>
        public bool IsSimulationFrameOver()
        {
            if (EmergencyExit()) { return false; }
            return CPlusWrapper.IsSimulationFrameOver();
        }

        /// <summary>
        /// Returns the X and Y joystick values for the given handController  (temporarily, x-flexion, y-abduction)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vector2 GetHandJoystickValues(int id)
        {
            if (EmergencyExit()) { return Vector2.zero; }
            CPlusWrapper.GetHandJoystickValues(id, out x, out y);
            //return new Vector3d(x, y, z).ToFloatVector();
            return Vector2.zero.Set(x, y);
        }




        // DEBUGING

        /// <summary>
        /// Get number of bones in hand
        /// </summary>
        /// <returns></returns>
        public int GetNBones(int handIndex)
        {
            return CPlusWrapper.GetNBones(handIndex);
        }

        /// <summary>
        /// Get number of bones in hand
        /// </summary>
        /// <returns></returns>
        public int GetNJoints(int handIndex)
        {
            return CPlusWrapper.GetNJoints(handIndex);
        }
        /// <summary>
        /// Get number of FEM nodes
        /// </summary>
        /// <returns></returns>
        public int GetNFEMNodes(int handIndex)
        {
            return CPlusWrapper.GetNFEMNodes(handIndex);
        }

        /// <summary>
        /// Returns the number of reference points in the hand. It should be one per device (for debugging)
        /// </summary>
        public int GetNRefPoints()
        {
            return CPlusWrapper.GetNRefPoints();
        }

        /// <summary>
        ///  Returns the number of fingertips in the hand
        /// </summary>
        /// <returns></returns>
        public int GetNPlatforms()
        {
            return CPlusWrapper.GetNPlatforms();
        }
        /// <summary>
        ///Returns the number of faces in the FEMmesh
        /// </summary>
        /// <returns></returns>
        public int GetNFacesMesh(int handIndex)
        {
            return CPlusWrapper.GetNFacesMesh(handIndex);
        }
        /// <summary>
        /// Returns the total number of faces in all the FEM meshes
        /// </summary>
        /// <returns></returns>
        public int GetTotalNFacesMesh()
        {
            return CPlusWrapper.GetTotalNFacesMesh();
        }


        /// <summary>
        ///Returns the number of vertices in the FEMmesh*/
        /// </summary>
        /// <returns></returns>
        public int GetNVerticesMesh(int handIndex)
        {
            return CPlusWrapper.GetNVerticesMesh(handIndex);
        }
        /// <summary>
        /// Returns the total number of all the verticies in all the fem meshes
        /// </summary>
        /// <returns></returns>
        public int GetTotalNVerticesMesh()
        {
            return CPlusWrapper.GetTotalNVerticesMesh();
        }

        /// <summary>
        /// Get position of FEM node
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="position"></param>
        public void GetFEMNodePosition(int handIndex, int nodeId, ref Vector3 position)
        {
            if (EmergencyExit()) { position = new Vector3(); }
            CPlusWrapper.GetFEMNodePosition(handIndex, nodeId, out x, out y, out z);
            //position = new Vector3d(x, y, z).ToFloatVector();
            position.Set(x, y, z);

        }
        /// <summary>
        /// Get position of FEM node
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="position"></param>
        /// <param name="ratio"></param>
        public void GetFEMNodePosition(int handIndex, int nodeId, ref Vector3 position, out float ratio)
        {
            if (EmergencyExit()) { position = new Vector3(); ratio = 1f; }
            CPlusWrapper.GetFEMNodePositionWithRatio(handIndex, nodeId, out x, out y, out z, out ratiod);
            //position = new Vector3d(x, y, z).ToFloatVector();
            position.Set(x, y, z);
            ratio = (float)ratiod;
        }

        /// <summary>
        /// Get number of contacts
        /// </summary>
        /// <returns></returns>
        public int GetNContacts()
        {
            if (EmergencyExit()) { return 0; }
            return CPlusWrapper.GetNContacts();
        }

        /*Get contact info, location, normal, anchor and state*/
        public void GetContactInfo(int contactId, out Vector3 pos, out Vector3 n, out Vector3 anchor, out bool kinetic,
             out Vector3 contactForce, out Vector3 frictionForce)
        {

            double posX, posY, posZ, nX, nY, nZ, anchorX, anchorY, anchorZ, contactForceX, contactForceY, contactForceZ, frictionForceX, frictionForceY, frictionForceZ;
            CPlusWrapper.GetContactInfo(contactId, out posX, out posY, out posZ, out nX, out nY, out nZ, out anchorX, out anchorY, out anchorZ, out kinetic, out contactForceX, out contactForceY, out contactForceZ, out frictionForceX, out frictionForceY, out frictionForceZ);

            pos = new Vector3((float)posX, (float)posY, (float)posZ);
            n = new Vector3((float)nX, (float)nY, (float)nZ);
            anchor = new Vector3((float)anchorX, (float)anchorY, (float)anchorZ);
            contactForce = new Vector3((float)contactForceX, (float)contactForceY, (float)contactForceZ);
            frictionForce = new Vector3((float)frictionForceX, (float)frictionForceY, (float)frictionForceZ);

        }
        public int GetNFingertipVertices()
        {
            if (EmergencyExit()) { return 0; }
            return CPlusWrapper.GetNFingertipVertices();
        }
 
        /*Get FingertipContactInfo*/
        public void GetFingertipVertex(int i, out Vector3 pos, out bool isInContact)
        {
            double posX, posY, posZ;
            CPlusWrapper.GetFingertipVertex(i, out posX, out posY, out posZ, out isInContact);
            pos = new Vector3((float)posX, (float)posY, (float)posZ);
        }

        /// <summary>
        ///Get start and end position of bone, and its up vector
        /// </summary>
        /// <param name="boneId"></param>
        /// <param name="boneType"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void GetBonePositionsAndOrientation(int handIndex, int boneId, int boneType, ref Vector3 position, ref Quaternion rotation)
        {
            if (EmergencyExit()) { position = Vector3.zero; rotation = Quaternion.identity; return; }
    
            CPlusWrapper.GetBonePositionsAndOrientation(handIndex, boneId, boneType, out x, out y, out z, out rw, out rx, out ry, out rz);
            //position = new Vector3d(x, y, z).ToFloatVector();
            position = Vector3.zero.Set(x, y, z);

            //rotation = new Quaternion((float)rx, (float)ry, (float)rz, (float)rw); //TODO remember that unity quaternion is x,y,z,w   and it seems like clap is wxyz
            rotation = Quaternion.identity.Set(rx, ry, rz, rw);
        }


        /// <summary>
        /// Gets the current position of the joint
        /// </summary>
        /// <param name="jointId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Vector3 GetJointPosition(int handIndex, int jointId, int type)
        {
            if (EmergencyExit()) { return Vector3.zero; }
            CPlusWrapper.GetJointPosition(handIndex, jointId, type, out x, out y, out z);
            return Vector3.zero.Set(x,y,z);
            //return new Vector3d(x, y, z).ToFloatVector();
        }


        public bool TestGetOutputBool()
        {
            if (EmergencyExit()) { return false; }
            return CPlusWrapper.TestGetOutputBool();
        }

        /*public void UpdateHardCodedValues(ClapBehaviour.HCValues hcValues)
        {
            if (EmergencyExit()) { return; }
            CPlusWrapper.UpdateHardCodedValues(hcValues.maxSquaredDistance, hcValues.handStiffness, hcValues.handDamping, hcValues.nNeighbersKDTree, hcValues.femTriangleDivisions, hcValues.exponentialK, hcValues.exponentialPlateau, hcValues.useBarycentricLimitation);
        }*/

        public void EnableInterhandCollisions(bool enabled)
        {
            if (EmergencyExit()) { return; }

            CPlusWrapper.EnableInterhandCollisions(enabled);
        }



        #endregion


        [DllImport("CLAPDLL.dll")]
        public static extern void UpdateHardCodedValues(double maxSquaredDistance, double handStiffness, double handDamping, int nNeighboursKDTree, int femTriangleDivisions, double exponentialK, double exponentialPlateau, bool useBarycentricLimitation);



        #region C++ Functions
        /************************************************************
         *                      C++ Functions
         * **********************************************************/
        //c++ stuff


        public class CPlusWrapper
        {


            [DllImport("CLAPDLL.dll")]
            public static extern bool GetCLAPCSharp();
            [DllImport("CLAPDLL.dll")]
            public static extern bool DestroyCLAPCSharp();
            [DllImport("CLAPDLL.dll")]
            public static extern bool TerminateCLAPCSharp();
            [DllImport("CLAPDLL.dll")]
            public static extern void SetTrackingMethod(int trackingMethod);
            [DllImport("CLAPDLL.dll")]
            public static extern void SetTrackingOption(int handIndex, int optionName, bool option);
            [DllImport("CLAPDLL.dll")]
            public static extern void SetSimulationMode(int simulationMode);
            ///Creates N simulation hand instances
            [DllImport("CLAPDLL.dll")]
            public static extern void CreateNHands(int n);


            [DllImport("CLAPDLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool Initialize(int type, [MarshalAs(UnmanagedType.LPStr)] char[] resourcePath, bool debugRedirect, int numHands);


            [DllImport("CLAPDLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetDataSetExportPath([MarshalAs(UnmanagedType.LPStr)] char[] datasetExportPath);


            [DllImport("CLAPDLL.dll")]
            public static extern void EnableCoupling();
            [DllImport("CLAPDLL.dll")]
            public static extern void AddContactPlane(double px, double py, double pz, double nx, double ny, double nz);

            [DllImport("CLAPDLL.dll")]
            public static extern void SetHandPosition(int handIndex, double x, double y, double z);
            [DllImport("CLAPDLL.dll")]
            public static extern void SetHandOrientation(int handIndex, double w, double x, double y, double z);
            [DllImport("CLAPDLL.dll")]
            public static extern void GetHandPosition(int handIndex, out double x, out double y, out double z);
            [DllImport("CLAPDLL.dll")]
            public static extern void GetHandOrientation(int handIndex, out double w, out double x, out double y, out double z);


            [DllImport("CLAPDLL.dll")]
            public static extern void StartSimulation();
            [DllImport("CLAPDLL.dll")]
            public static extern void StopSimulation();

            [DllImport("CLAPDLL.dll")]
            public static extern void EnableSimulation();
            [DllImport("CLAPDLL.dll")]
            public static extern void DisableSimulation();

            [DllImport("CLAPDLL.dll")]
            public static extern int GetNVerticesSkin(int handIndex);
            [DllImport("CLAPDLL.dll")]
            public static extern int GetTotalNVerticesSkin();

            [DllImport("CLAPDLL.dll")]
            public static extern void GetVertexSkin(int handIndex, int vertex, out double x, out double y, out double z);

            [DllImport("CLAPDLL.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetVerticesSkin(int handIndex, out int count);

            [DllImport("CLAPDLL.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int FreeVerticesSkinMem(IntPtr ptr);


            [DllImport("CLAPDLL.dll")]
            public static extern void GetVertexMesh(int handIndex, int vertex, out double x, out double y, out double z);


            [DllImport("CLAPDLL.dll")]
            public static extern void GetUVSkin(int handIndex, int vertex, out double u, out double v);
            [DllImport("CLAPDLL.dll")]
            public static extern int GetNFacesSkin(int handIndex);
            [DllImport("CLAPDLL.dll")]
            public static extern int GetTotalNFacesSkin();

            [DllImport("CLAPDLL.dll")]
            public static extern void GetFaceSkin(int handIndex, int face, out int a, out int b, out int c);
            [DllImport("CLAPDLL.dll")]
            public static extern void GetFaceMesh(int handIndex, int face, out int a, out int b, out int c);
            [DllImport("CLAPDLL.dll")]
            public static extern void EnableTracking();
            [DllImport("CLAPDLL.dll")]
            public static extern void DisableTracking();
            [DllImport("CLAPDLL.dll")]
            public static extern void Reset();

            [DllImport("CLAPDLL.dll")]
            public static extern void LockScene();
            [DllImport("CLAPDLL.dll")]
            public static extern void UnlockScene();

            [DllImport("CLAPDLL.dll")]
            public static extern int AddRigidBody(double mass, double i11, double i22, double i33, double i21, double i31, double i32, double scale, int type, bool isStationary, int parentId = -1);

            [DllImport("CLAPDLL.dll")]
            public static extern void SetRigidBodyNVerticesAndFaces(int id, int nVertices, int nFaces);

            /*Sets a vertex of a rigid body in case it is not a predefined object*/
            [DllImport("CLAPDLL.dll")]
            public static extern void SetRigidBodyVertex(int bodyId, int vertexId, double x, double y, double z);

            /*Sets the indices of a face for a rigid body in case it is not a predefined object*/
            [DllImport("CLAPDLL.dll")]
            public static extern void SetRigidBodyFace(int bodyId, int faceId, int v0, int v1, int v2);
            [DllImport("CLAPDLL.dll")]
            public static extern void SetupRigidBody(int id);



            //Update Functions:
            [DllImport("CLAPDLL.dll")]
            public static extern int StepsFinished();
            [DllImport("CLAPDLL.dll")]
            public static extern void UpdateMesh();
            /*Gets the current position of the object (for debugging)*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetRigidBodyPosition(int id, out double x, out double y, out double z);
            /*Gets the current orientation of the object (for debugging)*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetRigidBodyOrientation(int id, out double w, out double x, out double y, out double z);

            /*Sets the current position of the object*/
            [DllImport("CLAPDLL.dll")]
            public static extern void SetRigidBodyPosition(int id, double x, double y, double z, bool init = false);

            /*Sets the current orientation of the object*/
            [DllImport("CLAPDLL.dll")]
            public static extern void SetRigidBodyOrientation(int id, double w, double x, double y, double z, bool init = false);
            /*Sets the coupling spring constants of the object*/
            [DllImport("CLAPDLL.dll")]
            public static extern void SetRigidBodyCouplingStiffness(int id, double linearStiffness, double angularStiffness, double damping, double angularDamping);


            /*Get position of FEM node*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetFEMNodePosition(int handIndex, int nodeId, out double x, out double y, out double z);
            [DllImport("CLAPDLL.dll")]
            public static extern void GetFEMNodePositionWithRatio(int handIndex, int nodeId, out double x, out double y, out double z, out double ratio);
            [DllImport("CLAPDLL.dll")]
            public static extern bool CouplingEnabled();
            [DllImport("CLAPDLL.dll")]
            public static extern bool ObjectIsInContact(int id);
            /*Gets the current force applied on the object*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetRigidBodyForce(int id, out double fx, out double fy, out double fz);
            /*Gets the current torque applied on the object*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetRigidBodyTorque(int id, out double x, out double y, out double z);


            /*Gets the current linear velocity of the object (for debugging)*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetRigidBodyLinearVelocity(int id, out double x, out double y, out double z);

            /*Gets the current angular velocity of the object (for debugging)*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetRigidBodyAngularVelocity(int id, out double x, out double y, out double z);

            /*Gets the current position of the proxy object (for debugging)*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetRigidBodyProxyPosition(int id, out double x, out double y, out double z);

            /*Gets the current orientation of the proxy object (for debugging)*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetRigidBodyProxyOrientation(int id, out double w, out double x, out double y, out double z, bool proxy);


            [DllImport("CLAPDLL.dll")]
            public static extern void UpdateFrictionStiffnessAndCoefficient(int id, double stiffness, double mu);
            [DllImport("CLAPDLL.dll")]
            public static extern void SetCouplingParams(int id, double linearStiffness, double angularStiffness, double linearDamping, double angularDamping);
            [DllImport("CLAPDLL.dll")]
            public static extern void LoadNextFrame();
            [DllImport("CLAPDLL.dll")]
            public static extern bool IsSimulationFrameOver();

            /*Returns the X and Y joystick values for the given handController  (temporarily, x-flexion, y-abduction)*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetHandJoystickValues(int handId, out double x,out double y);
            



            //Debugging:
            [DllImport("CLAPDLL.dll")]
            public static extern int GetNBones(int handIndex);
            [DllImport("CLAPDLL.dll")]
            public static extern int GetNJoints(int handIndex);
            [DllImport("CLAPDLL.dll")]
            public static extern int GetNFEMNodes(int handIndex);
            [DllImport("CLAPDLL.dll")]
            public static extern int GetNRefPoints();
            [DllImport("CLAPDLL.dll")]
            public static extern int GetNPlatforms();
            [DllImport("CLAPDLL.dll")]
            public static extern int GetNFacesMesh(int handIndex);
            [DllImport("CLAPDLL.dll")]
            public static extern int GetTotalNFacesMesh();
            [DllImport("CLAPDLL.dll")]
            public static extern int GetNVerticesMesh(int handIndex);
            [DllImport("CLAPDLL.dll")]
            public static extern int GetTotalNVerticesMesh();

            [DllImport("CLAPDLL.dll")]
            public static extern int GetNContacts();
            /*Get contact info, location, normal, anchor and state*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetContactInfo(int contactId, out double posX, out double posY, out double posZ, out double nX, out double nY, out double nZ, out double anchorX, out double anchorY, out double anchorZ, out bool kinetic,
                out double contactForceX, out double contactForceY, out double contactForceZ, out double frictionForceX, out double frictionForceY, out double frictionForceZ);

            [DllImport("CLAPDLL.dll")]
            public static extern int GetNFingertipVertices();
            [DllImport("CLAPDLL.dll")]
            public static extern void GetFingertipVertex(int i, out double posX, out double posY, out double posZ, out bool isInContact);




            /*Get start and end position of bone, and its up vector*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetBonePositionsAndOrientation(int handIndex, int boneId, int boneType, out double pX, out double pY, out double pZ, out double rW, out double rX, out double rY, out double rZ);
            /*Get position of joint by index*/
            [DllImport("CLAPDLL.dll")]
            public static extern void GetJointPosition(int handIndex, int jointId, int type, out double x, out double y, out double z);


            [DllImport("CLAPDLL.dll")]
            public static extern bool TestGetOutputBool();
            [DllImport("CLAPDLL.dll")]
            public static extern void UpdateHardCodedValues(double maxSquaredDistance, double handStiffness, double handDamping, int nNeighboursKDTree, int femTriangleDivisions, double exponentialK, double exponentialPlateau, bool useBarycentricLimitation);

            [DllImport("CLAPDLL.dll")]
            public static extern void EnableInterhandCollisions(bool enabled);

            /*
            #region unsued alternatives

            [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
            private delegate void NoParams();
            static NoParams EnableSimulation;


            [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
            private delegate IntPtr OnlyID(ref IntPtr aID, ref IntPtr aID2);

            [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
            private delegate IntPtr OneParam(ref IntPtr aID, float param);






            //https://thelazydev.net/blog/post/using-cc-within-c-part-2-writing-and-using-a-dll

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            private static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            #endregion
            */

        }
     }

    #endregion

}

