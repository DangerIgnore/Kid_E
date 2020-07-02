using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Clap
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(MyMesh))]
    public class InteractionObject : MonoBehaviour
    {

        public Rigidbody rb;
        public MyMesh mesh;
        public int id;
        public IOContactConfiguration contactConfiguration;

        public CouplingProxy proxy;
        public ClapBehaviour.ProxySetings proxySettings;
        public CouplingSpringConfiguration couplingSpringConfiguration;
        //public bool useChildScale;
        //public List<InteractionObject> childIOs;
        [HideInInspector]public ClapBehaviour clapBehaviour;
        Quaternion startRotation; //used for configurable joint setup
        //public int parentId=-1;//id of parent IO within clap
        // Use this for initialization
        void Awake()
        {
            if (rb == null) { rb = GetComponent<Rigidbody>(); }
            if (mesh == null) { mesh = GetComponent<MyMesh>(); }

       }

        public void SetFrictionCoefs()
        {
            clapBehaviour.CLAP.UpdateFrictionStiffnessAndCoefficient(id, contactConfiguration.stiffness, contactConfiguration.frictionCoefficient);
        }
        public void UpdateFrictionCoefs()
        {
            if (contactConfiguration.updateEveryFrame)
            {
                SetFrictionCoefs();
            }
        }

        public int GetNVerticies()
        {
            if (mesh == null) { mesh = GetComponent<MyMesh>(); }
            return mesh.GetMeshVertexCount();
        }
        public int GetNIndexes()
        {
            if (mesh == null) { mesh = GetComponent<MyMesh>(); }
            return mesh.GetNIndexes();
        }
        public void GetVerticies(List<Vector3> verticies)
        {
            mesh.GetVerticies(verticies);
        }
        public void GetIndices(List<int> indicies)
        {
            mesh.GetIndices(indicies);
        }

        public void SetVisibility(bool visible)
        {
            mesh.SetVisibility(visible);
        }


        #region EnablePhysicsSimulation
        private bool _physicsIsEnabled = true;
        public bool physicsIsEnabled
        {
            get { return _physicsIsEnabled; }
            set { SetSimulatePhysics(value); }
        }
        public void SetSimulatePhysics(bool b)
        {
            if (b) { Enable(); }
            else { Disable(); }
        }
        void Enable()
        {
            _physicsIsEnabled = true;

            if (rb == null) { rb = GetComponent<Rigidbody>(); }
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }
        void Disable()
        {
            _physicsIsEnabled = false;

            if (rb == null) { rb = GetComponent<Rigidbody>(); }
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        #endregion


        /// <summary>
        /// Used to turn off the proxy gameobject for the mirrored left hand
        /// </summary>
        public void DisableProxy()
        {
            proxy.gameObject.SetActive(false);
        }

        /// <summary>
        /// Fills all the values required automatically in the editor mode.
        /// </summary>
        public void EditorSetup()
        {
            rb = GetComponent<Rigidbody>();
            mesh = GetComponent<MyMesh>();

            mesh.Setup();
        }


        //Called by unity when adding the component or calling reset function.
        void Reset()
        {
            EditorSetup();
        }

        /*
        [System.Serializable]
        public class ConfigJointConstraints
        {
            public ConfigurableJointMotion[] PosXYZRotXYZ = new ConfigurableJointMotion[]{ ConfigurableJointMotion.Free, ConfigurableJointMotion.Free, ConfigurableJointMotion.Free, ConfigurableJointMotion.Free, ConfigurableJointMotion.Free, ConfigurableJointMotion.Free };

        }
        */
        [System.Serializable]
        public class CouplingSpringConfiguration
        {
            [SerializeField] public double linearStiffness = 177 * 20.0 * 1 * 1;
            [SerializeField] public double angularStiffness = 36.0 * 0.5;
            [SerializeField] public double linearDamping =0.0;
            [SerializeField] public double angularDamping = 0.0;
        }

        [System.Serializable]
        public class IOContactConfiguration
        {
            [SerializeField] public bool updateEveryFrame;
            [SerializeField]  public double stiffness=100.0;
            [SerializeField] public double frictionCoefficient=0.3; //mu
        }
        /// <summary>
        /// Passes the proxy settings from the interaction object to its corresponding proxy object in Unity
        /// </summary>
        public void PushProxyParams()
        {
            proxy.SetParams(proxySettings);
        }
        
        /// <summary>
        /// Returns true if the object is in contact with the hand
        /// </summary>
        /// <returns></returns>
        public bool IsInContact()
        {
            return clapBehaviour.CLAP.ObjectIsInContact(id);
        }
    }
}