using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;
using UnityEngine.XR;
using CLAP;

namespace Clap { 
public class ClapBehaviour : MonoBehaviour
{

    public int numberHands = 2;
    public float scale = 1;  //if clapsize= realife 20cm hand,   Unity=clapSize*scale = 20cm*15=3m hand 

    public IClapWrapper CLAP;
    bool initialized = false;

    string m_dllPath;
    string m_resourcePath;


        #region Variables From actorclap.h
        /************************************************************
         *                      Variables
         * **********************************************************/
        int nRefPoints;
    int nPlatforms;

    public ClapOptions clapOptions;
    public ClapDebugging clapDebugging;
    [HideInInspector] public ClapConfigSO clapConfigSO;
    [HideInInspector] public ClapAuxPrefabs prefabs;
   
    //duplicated vertices
    int[][] doubles;
    int[][] doublesFEM;

    Vector3 handPos;
    Quaternion handOrientation;

    // Transformation between CLAP and Unreal reference systems
    Quaternion transf = new Quaternion(0.5f * Mathf.Sqrt(2), 0f, 0f, 0.5f * Mathf.Sqrt(2));

    double timeStretch = 0.0;
    int lastCLAPStep = 0;


    // List of object in scene simulating physics
    protected List<InteractionObject> interactionObjects;
    List<Vector3> lastVelocity;
    List<Vector3> lastAngularVelocity;
    List<Vector3> lastPosition;
    List<Vector3> lastPosition2;
    List<Quaternion> lastOrientation;
    List<Vector3> lastOrientationE;
    List<Vector3> lastPredictedPosition;
    List<Quaternion> lastPredictedOrientation;

    //Debugging Lists:
    List<GameObject> handBones;
    List<GameObject> handJoints;


    //Variables to overwrite, not created and destroyed continuosly
    Vector3 coordinates;
    Vector3 coordinatesUV;
    Quaternion quatRotation;



        #endregion

    // https://stackoverflow.com/questions/8836093/how-can-i-specify-a-dllimport-path-at-runtime

    protected List<HandInstance> hands;
    HandInstance hand; //reference to current hand to work with in for loops.
    GameObject initialMesh;   
        void OnApplicationQuit()
        {

            if (CLAP == null)
            {
                Debug.Log("Clap has already been destroyed");
                return;
            }

            Debug.Log("End Runtime");
            CancelInvoke();


            CLAP.TerminateClap();

          }

        void Quit()
    {
        CLAP.DestroyCLAP();

    }

    //Singleton
    public static ClapBehaviour Instance;
    private void Awake()
    {
            if (numberHands < 1 || numberHands > 2)
            {
                numberHands = Mathf.Clamp(numberHands, 1, 2);
                Debug.Log("Clap currently only supports 1 or 2 hands, the number of hands has been clamped to: "+numberHands);
            }

         //Hardcoded Assests/Clap/...   Modify this if User moves the Clap folder
            m_dllPath = Application.dataPath + "/Clap/Core/Dlls";   
         m_resourcePath = Application.dataPath + "/Clap/Core/Dlls/resources";
         initialMesh = transform.GetChild(0).gameObject;
        if (clapConfigSO == null)
        {

            clapConfigSO = Resources.Load<ClapConfigSO>("Core/ClapConfiguration"); //in Clap/Core/Resources/Core
            if (clapConfigSO == null)
            {
                Debug.LogError("Configuration Scriptable object is necesary, Go To ProjectSettings/Clap to set up.");
                return;
            }
        }

        //Quickly override in public version for more transperancy
        clapConfigSO.DllFolderPath = m_dllPath;
        clapConfigSO.ResourcePath = m_resourcePath;
        clapConfigSO.DatasetExportPath = "";


            if (prefabs == null)
        {
            prefabs = Resources.Load<ClapAuxPrefabs>("Core/ClapPrefabHolder"); //in Clap/Core/Resources/Core
            if (prefabs == null)
            {
                Debug.LogError("Core Prefabs Holder is missing!");
                return;
            }
        }
        if (Instance)
        {
            Destroy(this);
            Debug.Log("There can only be one clap behaviour in the scene.");
            return;
        }
        else
        {
            Instance = this;
        }


        //Setup the Dll Directory in clap
        SetDllDirectory(clapConfigSO.GetDllFolder());

        //only do this in Build, not in Editor
#if !UNITY_EDITOR
            //set the current directory to be that of the dlls to allow build to find the dlls at runtime and not copy all the dlls into the exe folder.
            //See RBT's answer down below   https://stackoverflow.com/questions/8836093/how-can-i-specify-a-dllimport-path-at-runtime
            Directory.SetCurrentDirectory(clapUNConfig.GetDllFolder());
#endif

    }
    Vector3 XRheadPos;

    void Start()
    {

        //Protects from opening a non-existing dll
        if (clapConfigSO == null)
        {
            Debug.LogError("Configuration Scriptable object is necesary, Go To ProjectSettings/Clap to set up.");
            return;
        }

            DoOnStart();
        
        // ICLAP.	
        CLAP = new IClapWrapper(this);
        if (!CLAP.EmergencyExit())
        {

            /*Set tracking method */
            CLAP.SetTrackingMethod((int)clapConfigSO.TRACKING_METHOD);
            CLAP.SetSimulationMode((int)clapDebugging.simulationMode);
            //Initialize the chosen simulation mode
            SetupSimulationMode();

            CLAP.SetDataSetExportPath(clapConfigSO.DatasetExportPath);

            initialized = CLAP.Initialize(0, clapConfigSO.ResourcePath, clapDebugging.debugRedirectTextFile, numberHands);
            if (!initialized)
            {
               Debug.Log("Clap Not properly initialized.");
               return;
            }

            if (clapOptions.experimentalCoupling)
            {
                CLAP.EnableCoupling();
            }

            //enable or not the interhand collision detection
            CLAP.EnableInterhandCollisions(clapOptions.experimentalInterhandCollisions);

            Debug.Log("Done Initialization, Start Simulation");

            //CreateHand Instances in Unity:
            CreateHands();



            /*Standard ground plane*/
            float oneOverScale = 1.0f / scale;
            Vector3d center, normal;
            if (clapOptions.addFloorContactPlane)
            {
                center = ConvertVectorFromToCLAP(clapOptions.floorPlane.center, oneOverScale).ToDoubleVector();
                normal = ConvertVectorFromToCLAP(clapOptions.floorPlane.normal, 1f).ToDoubleVector().normalized;
                CLAP.AddContactPlane(center.x, center.y, center.z, normal.x, normal.y, normal.z);
                Debug.Log("Floor ContactPlane: " + center + "," + normal);
            }
            //Aditional wall contact planes
            if (clapOptions.addContactPlanes)
            {
                foreach (ClapContactPlane plane in clapOptions.contactPlanes)
                {
                    center = ConvertVectorFromToCLAP(plane.center, oneOverScale).ToDoubleVector();
                    normal = ConvertVectorFromToCLAP(plane.normal,1f).ToDoubleVector().normalized;
                    CLAP.AddContactPlane(center.x, center.y, center.z, normal.x, normal.y, normal.z);
                    //Debug.Log("ContactPlane: " + center + "," + normal);
                }
            }

              

                // Initialize HMD   //TODO
                if (XRDevice.isPresent)
            {
                UnityEngine.XR.InputTracking.Recenter();

                XRheadPos = Quaternion.Inverse(InputTracking.GetLocalRotation(XRNode.Head)) * InputTracking.GetLocalPosition(XRNode.Head);

                //Tell CLAP that HMD is going to be used
                for (int i = 0; i < numberHands; i++)
                {
                    CLAP.SetTrackingOption(i, IClapWrapper.CLAPTrackerOption.CLAP_TRACKER_HMD_MODE, true);
                }

      
                if (true)
                {
                    XRPlayer = Camera.main.transform;

                    XRPlayer.rotation = Quaternion.Euler(0, 0, 0);
                }
            }





#if true
            // Create ProceduralMesh
            /*Set position and orientation of the hand given the position and orientation in Unity*/
            handPos = ConvertVectorFromToCLAP(transform.position, 1.0f / scale);
            handOrientation = ConvertQuatFromToCLAP(transform.rotation);
            //Debug.Log("HandPos:" + handPos);
            //Debug.Log("handOrientation:" + handOrientation);

            for (int h = 0; h < numberHands; h++)
            {
                CLAP.SetHandPosition(h, handPos);
                CLAP.SetHandOrientation(h, handOrientation);
            }
    #endif

           
            //Initialize the GraphicsMesh
            InitializeMesh();

            InitializeInteractionObjects();

            if (clapDebugging.debug)
                {
                    nRefPoints = CLAP.GetNRefPoints();
                    nPlatforms = CLAP.GetNPlatforms();
                    Debug.Log("Debugging enabled, create bones and nodes for visual feedback");

                    for (int h = 0; h < numberHands; h++)
                    { //get number of hands
                        hand = hands[h];

                        hand.nBones = CLAP.GetNBones(h);
                        hand.nNodes = CLAP.GetNFEMNodes(h);

                        hand.nJoints = CLAP.GetNJoints(h);

                        Debug.Log("Hand " + h + " has " + hand.nBones + " bones.");
                        Debug.Log("Hand " + h + " has " + hand.nNodes + " nodes.");
                        Debug.Log("Hand " + h + " has " + hand.nJoints + " joints.");


                        Debug.Log("Hand " + h + " has " + CLAP.GetNFacesMesh(h) + " mesh faces.");
                        Debug.Log("Hand " + h + " has " + CLAP.GetNFacesSkin(h) + " skin faces.");
                    }


                    //////////BONES///////////////////////////////////////////////////

                    if (clapDebugging.debugShowBones)
                    {
                        CreateUnityClapBoneVisualization(Bonetype.SIMULATED_BONE);
                    }

                    if (clapDebugging.debugShowTrackedBones)
                    {
                        CreateUnityClapBoneVisualization(Bonetype.TRACKED_BONE);
                    }

                    /*ALWAYS CREATE THE JOINTS, then update them or not.*/
                    for (int h = 0; h < numberHands; h++)
                    {
                        hand = hands[h];

                        hand.handJoints = new List<GameObject>();
                        GameObject currentJoint;
                        GameObject jointHolder;
                        //Create a joint holder object as a child of the hand instance transform
                        jointHolder = new GameObject();
                        jointHolder.name = "JointHolder_hand_" + h;
                        jointHolder.transform.SetParent(hand.transform);
                        //instanciate the joint prefabs
                        for (int i = 0; i < hand.nJoints; i++)
                        {
                            // Debug.Log("Joint: " + i);
                            currentJoint = Instantiate(prefabs.p_joint) as GameObject;
                            currentJoint.name = "Joint_" + i;

                            currentJoint.transform.localScale = 0.01f * Vector3.one * scale;
                            hand.handJoints.Add(currentJoint);
                            currentJoint.transform.SetParent(jointHolder.transform);

                            currentJoint.SetActive(false); //invisible at first
                        }
                    }

                   ////////////////FEM MESH/////////////////////////////////////////////

                    InitializeFEMMesh();

                    bool showFemAtStart = clapDebugging.debugShowFem;
                    for (int h = 0; h < numberHands; h++)
                    {
                        hands[h].mesh.Enable(!showFemAtStart);
                        hands[h].meshFEM.Enable(showFemAtStart);
                    }


                }
                else
                {
                    //Debug.Log("No debugging enabled");
                    //meshFEM.Enable(false);
                    for (int h = 0; h < numberHands; h++)
                    {
                        hands[h].meshFEM.Enable(false);
                    }
                }


            DoAfterInitialization();

            /*Start hand simulation*/
            CLAP.StartSimulation();

            // Activate tracking
            CLAP.EnableTracking();

        
            int count;
            CLAP.GetVerticesSkin(0,  out count);


            lastCLAPStep = 0;
        }
        else
        {
            Debug.LogError(("Couldnt get CLAP entry point"));
            return;
        }

    }


    private void Update()
    {
        if (clapDebugging.simulationMode ==SimulationMode.ONLY_SIMULATION || clapDebugging.simulationMode ==SimulationMode.RUNTIME_RECORDING_STATES)
        {
            MyUpdate();
        }   
    }


    // Update is called once per frame
    void MyUpdate()
    {

        if (CLAP == null)
        {
            Debug.LogError("No clap dll found at Path: " + clapConfigSO.DllFolderPath);
            return;
        }

         if (!initialized)
         {
            Debug.LogError("Clap not initialized, check resource folder: " + clapConfigSO.ResourcePath);
            return;
         }

        timeStretch += Time.deltaTime;

        //TODO: KeyboardInput
        UpdateKeyboardInput();

        //Hardcoded values 
        IClapWrapper.UpdateHardCodedValues(clapDebugging.hcValues.maxSquaredDistance, clapDebugging.hcValues.handStiffness, clapDebugging.hcValues.handDamping, clapDebugging.hcValues.nNeighbersKDTree, clapDebugging.hcValues.femTriangleDivisions, clapDebugging.hcValues.exponentialK, clapDebugging.hcValues.exponentialPlateau, clapDebugging.hcValues.useBarycentricLimitation);
                       
        int clapStep = CLAP.StepsFinished();
        if (!(clapStep - lastCLAPStep < 1))
        {
            lastCLAPStep = clapStep;
        }


        //enable or not the interhand collision detection
        CLAP.EnableInterhandCollisions(clapOptions.experimentalInterhandCollisions);


        //TODO HMD UPDATES
        UpdateLocomotion();
        UpdateHMD();

        //SimulationMesh
        CLAP.UpdateMesh();

        ShowSkin(clapDebugging.debugShowSkin);

        if (clapDebugging.debug && clapDebugging.debugShowFem)
        {
            UpdateFEMMesh();
        }
        else
        {
            UpdateMesh();
        }
    
        //InteractionObjects in Scene
        UpdateInteractionObjects();

        //Debugging
        UpdateDebugging();


        //for overwriting
        DoAtEndOfUpdate();


        timeStretch = 0.0;
        //Debug.Log("Updating");
    }

        private void ShowSkin(object debugShowSkin)
        {
            throw new NotImplementedException();
        }


        #region Auxiliar Functions
        Vector3 axis, finalVector;
        float angle,swap;
        void ConvertQuatFromToCLAP(Quaternion initial, out Quaternion final)
    {
        initial.ToAngleAxis(out angle, out axis);

        //Swap x and z
        axis.Set(axis.z, axis.y, axis.x); //UNITY  
        angle *= -1.0f;

        final = Quaternion.AngleAxis(angle, axis);
      
        //normalize the quaternion:
        float Length = (float)System.Math.Sqrt(final.x * final.x + final.y * final.y + final.z * final.z + final.w * final.w);
        float scale = 1.0f / Length;
        final.x *= scale;
        final.y *= scale;
        final.z *= scale;
        final.w *= scale;
            
    }

    public Quaternion ConvertQuatFromToCLAP(Quaternion initial)
    {
        Quaternion final;
        ConvertQuatFromToCLAP(initial, out final);
        return final;
    }
    void ConvertVectorFromToCLAP(Vector3 initial, out Vector3 final, float _scale = 1.0f)
    {   
        final = initial * _scale;
        Swap(ref final.x, ref final.z);

    }
    public Vector3 ConvertVectorFromToCLAP(Vector3 initial, float _scale = 1.0f)
    {
        //Vector3 final;
        ConvertVectorFromToCLAP(initial, out finalVector, _scale);
        return finalVector;
    }
    public Vector3 ConvertVectorFromToCLAP(float x, float y, float z, float _scale = 1.0f)
    {
            finalVector.Set(z, y, x); //including the swap
            finalVector *= _scale;
            return finalVector;
    }
    public Vector3 ConvertVectorFromToCLAP(double x, double y, double z, float _scale = 1.0f)
    {
        return ConvertVectorFromToCLAP((float)x, (float)y, (float)z, _scale);
    }
        void ConvertTorqueFromToCLAP(Vector3 initial, out Vector3 final)
    {
        final = -initial;
        Swap(ref final.y, ref final.z); 

    }
    Vector3 ConvertTorqueFromCLAP(Vector3 initial)
    {
        ConvertTorqueFromToCLAP(initial, out finalVector);
        return finalVector;
    }
    void Swap(ref float a, ref float b)
    {
        swap = a;
        a = b;
        b = swap;
    }
    /// <summary>
    /// Translates from double precision to float precision
    /// and sets the transform with float values
    /// </summary>
    /// <param name="position"></param>
    void SetTransformPosition(Vector3d position)
    {
            transform.position.Set(position.x, position.y, position.z);// = new Vector3((float)position.x, (float)position.y, (float)position.z);
    }
    /// <summary>
    /// Transforms a Unity Vector3 of floats to a CLAP vector3d of Doubles
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    Vector3d FloatToDouble(Vector3 vector)
    {
        return new Vector3d(vector.x, vector.y, vector.z);
    }
    /// <summary>
    /// Transforms a Clap Vector3d of Doubles to a CLAP vector3 of floats
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    Vector3 DoubleToFloat(Vector3d vector)
    {
        return new Vector3((float)vector.x, (float)vector.y, (float)vector.z);
    }
   
  
    /// <summary>
    /// Creates and Initializes the Similation Graphics Mesh.
    /// </summary>
    void InitializeMesh()
    {

        //todo get number of hands
        for (int h = 0; h < numberHands; h++)
        {
            hand = hands[h];


            //Lists to fill and variables to overwrite in each loop of the for.
            hand._vertices = new List<Vector3>();
            hand._UV0 = new List<Vector2>();


            hand.nVerticesSkin = CLAP.GetNVerticesSkin(h);
            for (int i = 0; i < hand.nVerticesSkin; ++i)

            {
                // Vertex position
                coordinates =CLAP.GetVertexSkin(h, i);

               
                hand._vertices.Add(ConvertVectorFromToCLAP(coordinates, scale));

                // UV
                //double u, v;
                coordinatesUV = CLAP.GetUVSkin(h, i);
                hand._UV0.Add(coordinatesUV);
            }

            //Triangles
            int faceCount = CLAP.GetNFacesSkin(h);
            hand._triangles = new int[faceCount * 3];
            int[] triangle = { 0, 0, 0 };
            int q = 0;
            for (int i = 0; i < faceCount; i++)
            {
                triangle = CLAP.GetFaceSkin(h, i);
                q = 3 * i;

                //its necesary to put them in backwards, otherwise unity paints the hand inside out
                hand._triangles[q] = triangle[2];
                hand._triangles[q + 1] = triangle[1];
                hand._triangles[q + 2] = triangle[0];
            }

       
            hand.mesh.SetLists(hand._vertices, hand._triangles, hand._UV0);  

          
        }
            

        initialMesh.SetActive(false);

    }

    void InitializeFEMMesh()
    {
        //todo get number of hands
        for (int h = 0; h < numberHands; h++)
        {
            hand = hands[h];
            hand._verticesFEM = new List<Vector3>();
            hand._UV0FEM = new List<Vector2>();

            int vertexCountFEM = CLAP.GetNVerticesMesh(h);
            Debug.Log("vertexCountFEM = " + vertexCountFEM);
            for (int i = 0; i < vertexCountFEM; ++i)
            {
                // Vertex position
                coordinates = CLAP.GetVertexMesh(h, i);
                hand._verticesFEM.Add(ConvertVectorFromToCLAP(coordinates, scale));
                coordinatesUV = Vector2.one;  
                hand._UV0FEM.Add(coordinatesUV);
            }

            //Triangles
            int faceCountFEM = CLAP.GetNFacesMesh(h);
            hand._trianglesFEM = new int[faceCountFEM * 3];
            int[] triangle = { 0, 0, 0 };
            int q = 0;
            for (int i = 0; i < faceCountFEM; i++)
            {

                triangle = CLAP.GetFaceMesh(h, i);
                q = 3 * i;

                //its necesary to put them in backwards, otherwise unity paints the hand inside out
                hand._trianglesFEM[q] = triangle[2];
                hand._trianglesFEM[q + 1] = triangle[1];
                hand._trianglesFEM[q + 2] = triangle[0];
            }

            hand.meshFEM.SetLists(hand._verticesFEM, hand._trianglesFEM, hand._UV0FEM);

        }
    }

    void InitializeInteractionObjects()
    {
        CLAP.LockScene();


        InteractionObject[] objects_test = FindObjectsOfType<InteractionObject>();
        if (objects_test != null)
        {

            //Variables to use later
            interactionObjects = new List<InteractionObject>();
            lastVelocity = new List<Vector3>();
            lastAngularVelocity = new List<Vector3>();
            lastPosition = new List<Vector3>();
            lastPosition2 = new List<Vector3>();
            lastOrientation = new List<Quaternion>();
            lastOrientationE = new List<Vector3>();
            lastPredictedPosition = new List<Vector3>();
            lastPredictedOrientation = new List<Quaternion>();

            //Rigidbody body;
            int objectId;
            Vector3 inertiaTensor;
            double lscale;
            int typeSphere;
            bool isStationary;
            int numberVerticies, numberIndicies, numberFaces;
            List<Vector3> verticies = new List<Vector3>();
            List<int> indicies = new List<int>();

            float oneOverScale = 1.0f / scale;

            foreach (InteractionObject io in objects_test)
           
            {
                //Debug.Log("interaction object found");
                io.clapBehaviour = this;

                    if (io.physicsIsEnabled)
                {
                    //Add the InteractionObject to CLAP
                    objectId = interactionObjects.Count;
                    interactionObjects.Add(io);
                    io.id = objectId;

                    //Create storage for last linear and angular velocities;
                    lastVelocity.Add(Vector3.zero);
                    lastAngularVelocity.Add(Vector3.zero);
                    lastPosition.Add(io.transform.position);
                    lastPosition2.Add(io.transform.position);
                    lastOrientation.Add(io.transform.rotation);
                    lastOrientationE.Add(io.transform.eulerAngles);
                    lastPredictedPosition.Add(io.transform.position);
                    lastPredictedOrientation.Add(io.transform.rotation);


                    //CLAP.LockScene();

                    inertiaTensor = io.rb.inertiaTensor;
                    lscale = 0.1 / 8.0;
                    typeSphere = 4;
                    isStationary = (io.rb.constraints == RigidbodyConstraints.FreezeAll);   
                    
                    inertiaTensor /= (scale * scale);

                    objectId = CLAP.AddRigidBody((double)io.rb.mass, (double)inertiaTensor.x, (double)inertiaTensor.z, (double)inertiaTensor.y, 0.0, 0.0, 0.0, lscale, typeSphere, isStationary);
                    io.id = objectId;

                    if (typeSphere == 4)
                    {
                        //clear out lists from last object
                        verticies.Clear();
                        indicies.Clear();

                        io.GetVerticies(verticies);
                        io.GetIndices(indicies);

                        numberVerticies = verticies.Count;
                        numberIndicies = indicies.Count;
                        numberFaces = numberIndicies / 3;

                        //Debug.Log("Mesh has " + numberVerticies + " vertices");
                        //Debug.Log("Mesh has " + numberFaces + " faces");

                        CLAP.SetRigidBodyNVerticesAndFaces(objectId, verticies.Count, numberFaces); //NumberFaces = NumberIndicies/3


                        Vector3 cscale = /*io.useChildScale? io.transform.GetChild(0).lossyScale :*/  io.gameObject.transform.lossyScale;
                        cscale *= oneOverScale;

                        //Debug.Log("cScale: " + cscale);

                        for (int j = 0; j < numberVerticies; j++)
                        {
                            position = ConvertVectorFromToCLAP(Vector3.zero.Setf(verticies[j][0] * cscale[0], verticies[j][1] * cscale[1], verticies[j][2] * cscale[2]));
                            CLAP.SetRigidBodyVertex(objectId, j, position.x, position.y, position.z);
                        }

                        //for each face,  REMEMBER, the there are 3 indicies per face.
                        for (int j = 0; j < numberFaces; j++)
                        {
                            CLAP.SetRigidBodyFace(objectId, j, indicies[j * 3], indicies[j * 3 + 1], indicies[j * 3 + 2]);
                        }

                        CLAP.SetupRigidBody(objectId);

                    }

                     //Set InteractionObject Position and Rotation
                    CLAP.SetRigidBodyOrientation(objectId, ConvertQuatFromToCLAP(io.transform.rotation), true);
                    CLAP.SetRigidBodyPosition(objectId, ConvertVectorFromToCLAP(io.transform.position, 1.0f / scale), true);
                    
                    CLAP.SetRigidBodyCouplingStiffness(objectId, io.couplingSpringConfiguration);


                    if (CLAP.CouplingEnabled() && io.physicsIsEnabled)
                    {
                        /*Physics of objects computed by Unity*/
                        io.SetSimulatePhysics(true);

                        /*Setup proxy actor objects*/
                        GameObject proxyGO = Instantiate(prefabs.p_proxyCoupling) as GameObject;
                        CouplingProxy proxy = proxyGO.GetComponent<CouplingProxy>();
                        
                        /*Set position and orientation of proxy actor to that of the object, the proxy object must be movable.*/
                        proxyGO.transform.SetPositionAndRotation(io.transform.position, io.transform.rotation);
                       // proxyGO.transform.localScale = io.transform.localScale;
                        //proxyGO.transform.localScale = (io.useChildScale ? io.transform.GetChild(0).lossyScale : io.gameObject.transform.lossyScale);//io.transform.lossyScale;


                        /*Create the rigid body attached to the proxyGo. This rigid body should not be simulated, and not generate collisions*/
                        
                        proxy.AttachSpringJointToRB(io.rb);
                  
                        //Give the interaction object a reference to its proxy
                        io.proxy = proxy;
                        proxy.SetMyMesh(io.mesh);

                        io.mesh.SetVisibility(false);

                        proxy.IgnoreCollisionsWithObject(io);

                    }


                }
            }//foreach


            foreach (InteractionObject io in objects_test)
            {
                    io.SetFrictionCoefs();

                    //io.UpdateChildTransforms();
            }

        }

        CLAP.UnlockScene();

    }


    public List<InteractionObject> GetInteractionObjects()
    {
        return interactionObjects;
    }

    void CreateUnityClapBoneVisualization(Bonetype type)
    {
        for (int h = 0; h < numberHands; h++)
        {
            hand = hands[h];
            if (hand.handBones == null)
            {
                hand.handBones = new List<ClapBone>();
            }

            GameObject currentBone;
            GameObject boneHolder;
            //Create a bone holder object as a child of this transform
            boneHolder = new GameObject();
            boneHolder.name = type.ToString() + "_BoneHolder_hand_" + h;
            boneHolder.transform.SetParent(hand.transform);
            //instanciate the bone prefabs
            Material mat= clapDebugging.materialsAndColors.mat_simulatedBones;
                switch (type)
                {
                    case Bonetype.SIMULATED_BONE:
                        mat = clapDebugging.materialsAndColors.mat_simulatedBones;
                        break;
                    case Bonetype.TRACKED_BONE:
                        mat = clapDebugging.materialsAndColors.mat_trackedBones;
                        break;
                }
            ClapBone bone;
            for (int i = 0; i < hand.nBones; i++)
            {
                Debug.Log("BoneComponent: " + i);
                currentBone = Instantiate(prefabs.p_bone) as GameObject;
                currentBone.name = "Bone_" + i;
                bone = currentBone.GetComponent<ClapBone>();
                currentBone.transform.localScale = new Vector3(0.01f, 0.03f, 0.01f)*scale;// 0.01f * Vector3.one*scale;
                hand.handBones.Add(bone);
                currentBone.transform.SetParent(boneHolder.transform);
                bone.SetMaterial(mat);
            }
        }
    }





    /// <summary>
    /// Updates the Simulation Graphics Mesh
    /// </summary>
    void UpdateMesh()
    {
        for (int h = 0; h < numberHands; h++)
        {
            hands[h].UpdateMesh();
        }
    }
    void ShowSkin(bool on)
    {
        for (int h = 0; h < numberHands; h++)
        {
            hands[h].ShowSkin(on);
        }
    }


    void UpdateFEMMesh()
    {

        for (int h = 0; h < numberHands; h++)
        {
            hands[h].UpdateFEMMesh();
        }
      
    }


    void UpdateKeyboardInput()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            CLAP.Reset();
        }
    }



    //Rigidbody ioBody; //used to overwrite in UpdateInteracctionObjects each loop.
    void UpdateInteractionObjects()
    {
        foreach (InteractionObject io in interactionObjects)
        {

            io.UpdateFrictionCoefs();
             

                if (clapOptions.experimentalCoupling)
            {
                // Get body
                //ioBody = io.rb;

                bool inContact = false;
                if (CLAP.ObjectIsInContact(io.id))
                {
                    inContact = true;
                    io.proxy.EnableSpring(true);
                    io.PushProxyParams();
                    io.SetVisibility(true);
                    
                }
                else
                {
                    io.proxy.EnableSpring(false);
                    io.proxy.ClearParams();
                    io.SetVisibility(true);
                    io.proxy.SetVisibility(false);
                }

                // Read force
                Vector3 force = CLAP.GetRigidBodyForce(io.id);
                force = ConvertVectorFromToCLAP(force);

                // Read Torque
                Vector3 torque = CLAP.GetRigidBodyTorque(io.id);
                torque = ConvertTorqueFromCLAP(torque);

                float intFactor = 0.25f;

                //Get vectors
                Vector3 ppos = CLAP.GetRigidBodyPosition(io.id);
                Quaternion prot = CLAP.GetRigidBodyOrientation(io.id);
                Vector3 pvel = CLAP.GetRigidBodyLinearVelocity(io.id);
                Vector3 pangVel = CLAP.GetRigidBodyAngularVelocity(io.id);

                //Convert units
                ppos = ConvertVectorFromToCLAP(ppos, scale);
                prot = ConvertQuatFromToCLAP(prot);
                pvel = ConvertVectorFromToCLAP(pvel, scale);
                pangVel = ConvertVectorFromToCLAP(pangVel);

                float dt = 0.01f;
                Vector3 linearAcceleration = 1f * 0.5f * dt * (pvel - lastVelocity[io.id]);
                Vector3 angularAcceleration = 1f * 0.5f * dt * (pangVel - lastAngularVelocity[io.id]);

                Quaternion offsetRotation = new Quaternion(0, 0, 0, 1); 

                float angle = pangVel.magnitude;

                Quaternion pAngVelQuat = new Quaternion(pangVel.x, pangVel.y, pangVel.z, 0.0f);
                Quaternion qz = new Quaternion(0, 0, 0, 1);
                pAngVelQuat = pAngVelQuat.TimesFloat(0.5f * dt * intFactor);             
                Quaternion pAngAccQuat = new Quaternion(angularAcceleration.x, angularAcceleration.y, angularAcceleration.z, 0.0f);
                pAngAccQuat = pAngAccQuat.TimesFloat(0.5f);
                offsetRotation = qz.Add((pAngVelQuat.Add(pAngAccQuat.TimesFloat(0.0f)))) * qz;
                offsetRotation.Normalize();
                   
                //Set the proxy transform
                io.proxy.SetPosition(ppos + intFactor * (dt * pvel + linearAcceleration * 0)); 
                io.proxy.SetRotation(offsetRotation * prot); 

                lastVelocity[io.id] = pvel;
                lastAngularVelocity[io.id] = pangVel;


                Vector3 linVelocity = io.rb.velocity;
                Vector3 angVelocity = io.rb.angularVelocity*Mathf.Rad2Deg;


                position = io.transform.position;
                Vector3 lastPos = lastPosition[io.id];
                Vector3 lastPos2 = lastPosition2[io.id];

             
                Quaternion angularDispl = Quaternion.Inverse(lastOrientation[io.id]) * io.transform.rotation;  
                float ang = 0.0f;
                Vector3 axis;
                angularDispl.ToAngleAxis(out ang, out axis);

                lastPosition[io.id] = position;
                lastPosition2[io.id] = lastPos;
                lastOrientation[io.id] = io.transform.rotation;

                lastOrientationE[io.id] = io.transform.rotation.eulerAngles; 
                Vector3 prediction = lastPredictedPosition[io.id];

                 
                lastPredictedPosition[io.id] = position + intFactor * (position - lastPos);
                Quaternion predictedOrientation = Quaternion.Euler((1.0f + intFactor) * io.transform.rotation.eulerAngles - intFactor * lastOrientationE[io.id]);
                lastPredictedOrientation[io.id] = predictedOrientation;
               



                // Compute rotation due to angular velocity
                if (inContact)
                {
                   Quaternion offsetRotation2 = new Quaternion(0, 0, 0, 1);//(0, 0, 0, 1);

                    angle = angVelocity.magnitude;
                    if (angle > 1e-4)
                    {
                        
                        Quaternion angVelQuat = new Quaternion(angVelocity.x, angVelocity.y, angVelocity.z, 0.0f);
                        Quaternion Qz = new Quaternion(0, 0, 0, 1);
                       
                        angVelQuat = angVelQuat.TimesFloat(0.5f * dt);
                        offsetRotation2 = Qz.Add(angVelQuat * Qz);
                        offsetRotation2.Normalize();
                        offsetRotation2 = Qz;
                    }
                    Quaternion quat = ConvertQuatFromToCLAP(lastPredictedOrientation[io.id]);
                    CLAP.SetRigidBodyOrientation(io.id, quat, false);

                    Vector3 updatedPos = ConvertVectorFromToCLAP(lastPredictedPosition[io.id], 1.0f / scale);
                        if (io.transform.position.magnitude >= 200)
                        {
                            Debug.LogError("ToInfinityAndBeyond 1: position:" + updatedPos + ", velocity: " + io.rb.velocity);
                        }
                        CLAP.SetRigidBodyPosition(io.id, updatedPos, false);

                }
                else
                {
                    rotation = ConvertQuatFromToCLAP(io.transform.rotation); 
                    position = ConvertVectorFromToCLAP(io.transform.position + 0f * 0.01f * io.rb.velocity, 1.0f / scale);

                        if (io.transform.position.magnitude >= 200)
                        {
                            Debug.LogError("ToInfinityAndBeyond 2: position:" + position + ", velocity: " + io.rb.velocity);
                        }
                        CLAP.SetRigidBodyOrientation(io.id, rotation, false);
                    CLAP.SetRigidBodyPosition(io.id, position, false);
                }





            } //If Coupling
            else
            {   //No coupling
                
                // Update position
                coordinates = CLAP.GetRigidBodyPosition(io.id);

                    io.transform.position = ConvertVectorFromToCLAP(coordinates, scale);
                    
                // Update orientation
                quatRotation = CLAP.GetRigidBodyOrientation(io.id);
                io.transform.rotation =  ConvertQuatFromToCLAP(quatRotation);

            }

            //TODO this can be removed if the constants are "Constant",   for debugging, leave them in.
            CLAP.SetRigidBodyCouplingStiffness(io.id, io.couplingSpringConfiguration);

        }
    }

    Quaternion HMDRotationCorrection = Quaternion.Euler(0, 0, -90) * Quaternion.Euler(90, 0, 0);   
    Quaternion LMorientation;
    Vector3 LMposition;
    float leapMotionOffset_float;//LM 20cm in front of hmd
    Vector3 leapMotionOffset;
    Transform XRPlayer; //the transform of the UnityXR HMD 
    void UpdateHMD()
    {
        //HMD on?
        if (XRDevice.isPresent && clapOptions.useHMD)
        {

            //Compute LMotion position and orientation based on HMD position
            leapMotionOffset_float = 0.1f*scale;// 0.10f * scale;  //LM 20cm in front of hmd
           
            //simpler but wrong
            LMorientation = XRPlayer.rotation * HMDRotationCorrection;
            LMposition = XRPlayer.position + XRPlayer.forward * leapMotionOffset_float;


            //Leap mption coordinates in Unity
            Debug.DrawLine(LMposition, LMposition + XRPlayer.right * 3, Color.red);  // (1,0,0)
            Debug.DrawLine(LMposition, LMposition + XRPlayer.up * 3, Color.green);   //(0,1,0)
            Debug.DrawLine(LMposition, LMposition + XRPlayer.forward * 3, Color.blue);//(0,0,1)


            //Convert to clap coordinates
            LMorientation = ConvertQuatFromToCLAP(LMorientation);
            LMposition = ConvertVectorFromToCLAP(LMposition, 1.0f / scale);

            //Set hand orientation and position regarding hmd leap motion
            for (int i = 0; i < numberHands; i++)
            {
                CLAP.SetHandPosition(i, LMposition);
                CLAP.SetHandOrientation(i, LMorientation);
            }
        }
    }

        void UpdateLocomotion()
    {
        if (XRPlayer&&XRPlayer.parent)
        {
            XRPlayer.parent.position += 5f * XRPlayer.forward * Input.GetAxis("Vertical");
            XRPlayer.parent.Rotate(Vector3.up * (25f * Time.deltaTime * Input.GetAxis("Horizontal")));
        }
    }

    void UpdateDebugging()
    {
        if (clapDebugging.debug)
        {


            //Update Debug
            if (clapDebugging.debugShowBones)
            {
                UpdateBoneVisualizationInUnityByType(Bonetype.SIMULATED_BONE);
            }
             if (clapDebugging.debugShowTrackedBones)
            {
                    UpdateBoneVisualizationInUnityByType(Bonetype.TRACKED_BONE);
            }

                //For showing the joint positions in world space
                if (clapDebugging.debugShowJoints)
            {
                for (int h = 0; h < numberHands; h++)
                {
                    hand = hands[h];


                    for (int i = 0; i < hand.nJoints; i++)
                    {
                        hand.handJoints[i].SetActive(true); //visible if turned on

                        //Right hand
                        position = ConvertVectorFromToCLAP(CLAP.GetJointPosition(h, i, 0), scale);
                        hand.handJoints[i].transform.position = position;  

                    
                    }
                }

            } 
        }
    }

    

    /// <summary>
    /// Creates the hand Instances in unity and in clap.
    /// </summary>
    void CreateHands()
    {
        //create the hands in unity
        hands = new List<HandInstance>();
        for (int i = 0; i < numberHands; i++)
        {
            //create the prefab
            GameObject handInstanceGO = Instantiate(prefabs.p_handInstance) as GameObject;
            //rename the GameObject in the Jerarquia
            handInstanceGO.name = "Hand Instance " + i;
            //give it a new parent transform
            handInstanceGO.transform.SetParent(transform);
            //get reference hand instance component logic, IMPORTANT: THIS has to be on the gameobject, else error.
            hand = handInstanceGO.GetComponent<HandInstance>();
            //give the hand some initial values
           // int layer = i % 2 == 0 ? clapDebugging.rightHandLayerMask : clapDebugging.leftHandLayerMask; //8 is right hand layer for even indexes. 9 is left hand for odds
            hand.Initialize(this, i, 0/*(int)Mathf.Log(layer, 2)*/);

            //add to list of hands
            hands.Add(hand);

        }
    }

    /// <summary>
    /// call this function to tell the hand instances what hand index they are and give them a reference to this clapbehaviour.
    /// </summary>
    void InitializeHands()
    {
        //TODO 
        for (int h = 0; h < numberHands; h++)
        {
           // int layer = h % 2 == 0 ? clapDebugging.rightHandLayerMask : clapDebugging.leftHandLayerMask; //8 is right hand layer for even indexes. 9 is left hand for odds
            hands[h].Initialize(this, h, 0/*(int)Mathf.Log(layer, 2)*/);
        }
    }
               


    /// <summary>
    /// Sets this gameobject and all the children of it to a given layermask
    /// </summary>
    /// <param name="root"></param>
    /// <param name="layerMask"></param>
    public void SetLayersOfGameObject(Transform root, int layerMask)
    {
        for(int i=0;i<root.childCount;i++)
        {
            Transform child = root.GetChild(i);

            // Do something with child, then recurse.
            child.gameObject.layer = layerMask;
            SetLayersOfGameObject(child, layerMask);       
        }
    }





    public void LoadNextFrame()
    {
        CLAP.LoadNextFrame();
        MyUpdate();
    }

    public bool IsSimulationFrameOver()
    {
        return CLAP.IsSimulationFrameOver();
    }
    bool isFinished;
    bool export;

    IEnumerator WaitForSimulationFrameToFinish()
    {
        SimValues v = clapDebugging.simValues;
        yield return new WaitForSeconds(v.warmUpWaitTimeBeforeExporting);


        while (v.currentFrame < v.numFramesToExport)
        {

            LoadNextFrame();

            ///wait for clap to finish
            while (!isFinished)
            {
                isFinished = IsSimulationFrameOver();
                yield return new WaitForSeconds(v.isFinishedRefreshRate);//(0.1f);
            }

            //reset
            isFinished = false;

            //take snapshot if in export mode
            if (export)
            {
                Export();
            }

            //next frame value
            v.currentFrame++;

        }
    }

    // Use this for initialization
    void SetupSimulationMode()
    {

        export = false;

        //only exicute if in data set creation mode
        bool a = clapDebugging.simulationMode == ClapBehaviour.SimulationMode.OFFLINE_DATASET_CREATION;
        bool b = clapDebugging.simulationMode == ClapBehaviour.SimulationMode.SIMPLE_REPLAY;
        if (a || b)
        {
            SimValues v = clapDebugging.simValues;
            string path = System.IO.Path.Combine(clapConfigSO.DatasetExportPath, "right_hand_StateFileStream.txt");
            if (System.IO.File.Exists(path))
            {
                StreamReader stream = new StreamReader(path);
                int numFramesToExport_RightHand = (int)Tools.CountLines(stream);
                numFramesToExport_RightHand /= (8 * 16);  //16 rigidBody per hand, 4 and 4=8,  I dont know why I need the 2, maybe 32 rb per hand??
                v.numFramesToExport = numFramesToExport_RightHand;
            }
            else
            {
                Debug.LogError("No exportFile called " + path + " to read from");
                v.numFramesToExport = 0;
            }

            //only export in one mode
            export = a;

            //give a N second delay to gaurentee that clap has fully loaded up.
            StartCoroutine("WaitForSimulationFrameToFinish");
        }



    }
        /// <summary>
        /// Updates the bone visualization of a given type of bone in the Unity Editor 
        /// </summary>
        /// <param name="type"></param>
        void UpdateBoneVisualizationInUnityByType(Bonetype type)
        {

            int j = (int)type;
            int boneOffset = hand.nBones * j;
            int index;
            Vector3 posS;
            for (int h = 0; h < numberHands; h++)
            {
                hand = hands[h];
                boneOffset = hand.nBones * j;

                switch (type)
                {
                    case Bonetype.SIMULATED_BONE:
                        boneOffset = 0;
                        break;
                    case Bonetype.TRACKED_BONE:
                        if (clapDebugging.debugShowBones)
                        {
                            boneOffset = hand.nBones;
                        }
                        else
                        {
                            boneOffset = 0;
                        }
                        break;
                }

                
                for (int i = 0; i < hand.nBones; i++)
                    {
                        CLAP.GetBonePositionsAndOrientation(h, i, j, ref position, ref rotation);

                        posS = ConvertVectorFromToCLAP(position, scale);

                        index = boneOffset + i;

                        hand.handBones[index].transform.localPosition = posS;
                        hand.handBones[index].transform.rotation = ConvertQuatFromToCLAP(rotation);
                        hand.handBones[index].SetActive(true);
                }
            }
        }



        #endregion


        Transform[] armsArray;



        #region Generic Overwritten Functions
        public virtual void DoOnStart() { }
    public virtual void DoAfterInitialization()
    {
        // THIS WRIST HACK has to go AFTER the SetLayersOfGameObject() function just above
        if (clapOptions.showArms)
        {
            armsArray = new Transform[numberHands];


            for (int h = 0; h < numberHands; h++)
            {
                hand = hands[h];
                GameObject currentBone;

                //right hand
                if (h % 2 == 0)
                {
                    currentBone = Instantiate(prefabs.p_foreArm_Right) as GameObject;
                }
                else
                {
                    currentBone = Instantiate(prefabs.p_foreArm_Left) as GameObject;
                }

                armsArray[h] = currentBone.transform;
                currentBone.transform.SetParent(hand.transform); 
                                                   
                currentBone.transform.GetChild(0).localPosition *= scale;
                currentBone.transform.GetChild(0).localScale *= scale;

            }



        }

      
    }
    public virtual void DoAtEndOfUpdate() {
          
                //For showing arm hack
                if (clapOptions.showArms)
                {
                    for (int h = 0; h < numberHands; h++)
                    {
                        hand = hands[h];
                     
                        //Update position
                        position = CLAP.GetVertexSkin(h, 12600);
                        position = (ConvertVectorFromToCLAP(position, scale));
                        armsArray[h].localPosition = position;
                                       
                        //Get forward from two verticies in the triangle
                        Vector3 coordinates2 = CLAP.GetVertexSkin(h, 12599);
                        Vector3 posi2 = (ConvertVectorFromToCLAP(coordinates2, scale));
                        Vector3 forwardHack = posi2 - position;

                        //get triangle normal
                        int triangleId = 12600;
                        Vector3 TriangleNormal = hand.mesh.ComputeTriangleNormal(triangleId);

                        //left hand fix
                        if (h % 2 != 0)
                        {
                            forwardHack *= -1;
                        }
                        //get new look rotation
                        rotation = Quaternion.LookRotation(forwardHack, TriangleNormal);
                        //update rotation
                        armsArray[h].localRotation = rotation;

                }
            }
            

        }
    public virtual void Export() { Debug.Log("No dataset exporter is used in this ClapBehaviour component."); }
#endregion



    //Re-used throughout whole script Aux variables:
    Vector3 position;
    Quaternion rotation;
    Color color;

#region Gizmos:

    //showContact variables
    Vector3 contactPos, anchorPos, contactNormal, contactForce, frictionForce = new Vector3();
    Color contactColor = new Color(1, 0, 0, 0.4f);

    //showFem variables
    Vector3 femNodePosition;
    float femNodeRatio;
    Color femNodeColor = new Color(0, 1, 0, 0.4f);

    private void OnDrawGizmos()
        {

            if (CLAP == null) { return; }

            if (clapDebugging.debug)
            {

                if (clapDebugging.debugShowContact)
                {
                    int nContacts = CLAP.GetNContacts();

                    for (int i = 0; i < nContacts; i++)
                    {
                                                bool kinetic;

                        CLAP.GetContactInfo(i, out contactPos, out contactNormal, out anchorPos, out kinetic, out contactForce, out frictionForce);

                        Vector3 posC = ConvertVectorFromToCLAP(contactPos, scale);

                        Vector3 posA = ConvertVectorFromToCLAP(anchorPos, scale);

                        contactForce = ConvertVectorFromToCLAP(contactForce);

                        frictionForce = ConvertVectorFromToCLAP(frictionForce);

                        contactNormal = ConvertVectorFromToCLAP(contactNormal);

                        contactNormal.Normalize();

                        if (!kinetic)
                        {
                            if (!clapDebugging.debugShowContactLines)
                            {
                                Gizmos.color = clapDebugging.materialsAndColors.col_nonKinetic;
                                Gizmos.DrawSphere(posC,scale* 0.1f * clapDebugging.debugContactLineLength);
                                Gizmos.DrawCube(posA, Vector3.one * 0.25f * clapDebugging.debugContactLineLength);
                            }
                            else
                            {

                                Gizmos.color = clapDebugging.materialsAndColors.col_nonKinetic;
                                Gizmos.DrawLine(posA,posA+ contactNormal* scale * 1.0f * clapDebugging.debugContactLineLength);
                                Gizmos.DrawLine(posA,posC);
                                Gizmos.color = clapDebugging.materialsAndColors.col_contactForce;
                                Gizmos.DrawLine(posA, posA + contactForce * scale * 1.0f * clapDebugging.debugContactLineLength);
                                Gizmos.color = clapDebugging.materialsAndColors.col_frictionForce;
                                Gizmos.DrawLine(posA, posA + frictionForce * scale * 1.0f * clapDebugging.debugContactLineLength);
                            
                            }
                        }
                        else
                        {
                            if (!clapDebugging.debugShowContactLines)
                            {
                                Gizmos.color = clapDebugging.materialsAndColors.col_kinetic;
                                Gizmos.DrawSphere(posC, scale * 0.1f * clapDebugging.debugContactLineLength);
                                Gizmos.DrawCube(posA, Vector3.one * 0.25f * clapDebugging.debugContactLineLength); 

                            }
                            else
                            {

                                //same as unreal
                                Gizmos.color = clapDebugging.materialsAndColors.col_kinetic;
                                Gizmos.DrawLine(posA, posA + contactNormal * scale * 1.0f * clapDebugging.debugContactLineLength);
                                Gizmos.DrawLine(posA, posC);
                                Gizmos.color = clapDebugging.materialsAndColors.col_contactForce;
                                Gizmos.DrawLine(posA, posA + contactForce * scale * 1.0f * clapDebugging.debugContactLineLength);
                                Gizmos.color = clapDebugging.materialsAndColors.col_frictionForce;
                                Gizmos.DrawLine(posA, posA + frictionForce * scale * 1.0f * clapDebugging.debugContactLineLength);


                            }
                        }
                    }

                }


                if (clapDebugging.debugShowFemNodes)
                {
                    for (int h = 0; h < numberHands; h++)
                    {
                        hand = hands[h];
                        for (int i = 0; i < hand.nNodes; i++)
                        {
                            CLAP.GetFEMNodePosition(h, i, ref femNodePosition, out femNodeRatio);
                            femNodePosition = ConvertVectorFromToCLAP(femNodePosition, scale);
 
                            Gizmos.color = femNodeColor;
                            if (femNodeRatio > 0.0)
                            {
                                Gizmos.DrawSphere(femNodePosition, 0.005f * scale);
                            }
                            else
                            {
                                Gizmos.color = Color.white - femNodeColor;
                                Gizmos.DrawSphere(femNodePosition, 0.005f * scale);
                            }
                        }
                    }
                }


            }

        }

        private void OnDrawGizmosSelected()
        {
            //FloorPlane
            if (clapOptions.addFloorContactPlane)
            {
                clapOptions.floorPlane.DrawGizmos(transform);
            }
            if (clapOptions.addContactPlanes)
            {
                //Wall Contact Planes
                foreach (ClapContactPlane plane in clapOptions.contactPlanes)
                {
                    plane.DrawGizmos(transform);
                }
            }
        }
        //#endif
        #endregion


        #region PrivateClasses

        public enum SimulationMode
    {
        ONLY_SIMULATION = 0,
        RUNTIME_RECORDING_STATES = 1,
        SIMPLE_REPLAY = 2,
        OFFLINE_DATASET_CREATION = 3
    }

        public enum Bonetype
        {
            SIMULATED_BONE = 0,
            TRACKED_BONE = 1,
            UNCONSTRAINED_BONE = 2,
            PROXY_BONE = 3
        }

        [System.Serializable]
    public class ClapDebugging
    {
        public bool debug;

        [HideInInspector]public bool debugRedirectTextFile = false;
        public ClapDebuggingMaterialsAndColors materialsAndColors;
        public bool debugShowBones;
        public bool debugShowTrackedBones;
        public bool debugShowJoints;

        public bool debugShowSkin;
        public bool debugShowFem;
        public bool debugShowFemNodes;
 
        public bool debugShowContact;
        public bool debugShowContactLines;
        public float debugContactLineLength = 0.1f;

        [HideInInspector]public HCValues hcValues;
        [HideInInspector] public SimValues simValues;

            public SimulationMode simulationMode = SimulationMode.ONLY_SIMULATION;
            //public LayerMask rightHandLayerMask;
            //public LayerMask leftHandLayerMask;

        }

    [Serializable]
    public class ClapOptions
    {
        public bool useHMD;
        public bool experimentalCoupling;
        public bool experimentalInterhandCollisions = false;
        public bool showArms;
        public bool addFloorContactPlane = true;
        public ClapContactPlane floorPlane;
        public bool addContactPlanes = true;
        public ClapContactPlane[] contactPlanes;
    }

        
        [CreateAssetMenu(fileName = "Data", menuName = "Clap/PrefabsSO", order = 2)]
        [System.Serializable]
        public class ClapAuxPrefabs: ScriptableObject
        {
            public GameObject p_handInstance, p_bone, p_joint, p_proxyCoupling, p_foreArm_Left, p_foreArm_Right;
        }

        [Serializable]
        public class ClapDebuggingMaterialsAndColors
        {
            public Material mat_trackedBones, mat_simulatedBones;
            public Color col_contactForce = new Color(0, 0, 1, 0.4f);
            public Color col_frictionForce = new Color(1, 1, 1, 0.4f);
            public Color col_kinetic = new Color(0, 1, 0, 0.4f);
            public Color col_nonKinetic = new Color(1, 0, 0, 0.4f);
            public Color col_fingertipIsContact = new Color(0, 1, 0, 0.4f);
            public Color col_fingerTipIsNotContact = new Color(1, 0, 0, 0.4f);
        }

      

        [System.Serializable]
    public class MirroredLeftHandParams
    {
        public double stiffness = 1000;
        public double mu;
    }

    [System.Serializable]
    public class Transform3
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
    }

    [System.Serializable]
    public class ClapContactPlane
    {
        public Vector3 center=Vector3.zero;
        public Vector3 normal=Vector3.up;
        public bool drawGizmo=true;
        public Color gizmoColor = Color.blue;
        public float gizmoSize = 1f;

            internal void DrawGizmos(Transform t)
        {
            if (drawGizmo)
            {
                
                Quaternion rotation = Quaternion.LookRotation(t.TransformDirection(normal));
                Matrix4x4 trs = Matrix4x4.TRS(t.TransformDirection(center), rotation, Vector3.one);
                Gizmos.matrix = trs;
                Gizmos.color = gizmoColor;
                Gizmos.DrawCube(Vector3.zero, new Vector3(gizmoSize, gizmoSize, 0.0001f));
                Gizmos.DrawLine(Vector3.zero, Vector3.zero + Vector3.forward*gizmoSize*0.3f);
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.color = Color.white;

            }
        }
    }

    [System.Serializable]
    public class ProxySetings
    {
       public bool showProxyMesh;
      public MyJointDrive linearSpring=new MyJointDrive { m_SpringStiffness = 14160, m_Damper = 1, m_maximumForce = 3.402823f * Mathf.Pow(10f, 38f) } ;
       public MyJointDrive angularSpring= new MyJointDrive { m_SpringStiffness = 21600, m_Damper = 1, m_maximumForce = 3.402823f * Mathf.Pow(10f, 38f) };
    }

    //
    // Resumen:
    //     ///
    //     How the joint's movement will behave along its local X axis.
    //     ///
    [System.Serializable]
    public class MyJointDrive
    {
        //
        // Resumen:
        //     ///
        //     Strength of a rubber-band pull toward the defined direction. Only used if mode
        //     includes Position.
        //     ///
        public float m_SpringStiffness=1000;
       
        //
        // Resumen:
        //     ///
        //     Resistance strength against the Position Spring. Only used if mode includes Position.
        //     ///
        public float m_Damper=1;
        //
        // Resumen:
        //     ///
        //     Amount of force applied to push the object toward the defined direction.
        //     ///
        public float m_maximumForce = 3.402823f * Mathf.Pow(10f, 38f);


        public JointDrive GetJointDriveEquivilent()
        {
            return new JointDrive() { positionSpring = m_SpringStiffness, positionDamper= m_Damper, maximumForce=m_maximumForce };
        }
    }

    [System.Serializable]
    public class HCValues
    {
       public double maxSquaredDistance = 0.0004;
       public double handStiffness = 1000;
        public double handDamping = 100;
       public int nNeighbersKDTree = 100;
        public int femTriangleDivisions = 1;
        public double exponentialK =0.2;
        public double exponentialPlateau=100000;
        public bool useBarycentricLimitation = true;
    }
    [System.Serializable]
    public class SimValues{
        public int numFramesToExport = 500;
        public float warmUpWaitTimeBeforeExporting = 0f;
        public float isFinishedRefreshRate = 0.016f; //check if simulation is finished every x seconds  30fps

        public int currentFrame;
    }


    //Awake  
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool SetDllDirectory(string lpPathName);

#endregion
        
    }

}