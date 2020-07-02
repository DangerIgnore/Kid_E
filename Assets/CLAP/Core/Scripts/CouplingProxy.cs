using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clap
{
    //[RequireComponent(typeof (SpringJoint))]
    //[RequireComponent(typeof(ConfigurableJoint))]

    [RequireComponent(typeof(JointToggler))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(MyMesh))]
    public class CouplingProxy : MonoBehaviour
    {

        Rigidbody realRb;
        //SpringJoint springJoint;
        [SerializeField]
        ConfigurableJoint configurableJoint;
        [SerializeField]
        private JointToggler jointToggler; //allows enabling and disabling the springjoint
        [SerializeField]
        MyMesh myMesh;


        public float stiffness = 177f * 100.0f;
        public float damping = 1.0f;
        public float maxDrive = 100000f;

        private void Awake()
        {
            //rb = GetComponent<Rigidbody>();
            //springJoint = GetComponent<SpringJoint>();

            Reset();
        }

        public void AttachSpringJointToRB(Rigidbody other)
        {
            this.realRb = other;

            //springJoint.connectedBody = other;
            configurableJoint.connectedBody = other;
        }

        /// <summary>
        /// Gets the transform of the body that the proxy is connected to.
        /// Can Return NULL if there is no connected body at the moment (not in contact)
        /// </summary>
        /// <returns></returns>
        public Transform GetConnectedBodyTransform()
        {
            /*
            if (springJoint.connectedBody)
            {
                return springJoint.connectedBody.transform;
            }*/
            if (configurableJoint.connectedBody)
            {
                return configurableJoint.connectedBody.transform;
            }
            return null;
        }
        public void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }
        public void SetRotation(Quaternion rot)
        {
            transform.rotation = rot;
        }

        /*
        public void SetParams(float stiffness,float damping, float maxDrive)
        {
            this.stiffness = stiffness;
            this.damping = damping;
            this.maxDrive = maxDrive;

            springJoint.spring = stiffness;
            springJoint.damper = damping;
            springJoint.breakForce = maxDrive;
        }
        */

        JointDrive linearDrive, angularDrive, slerpDrive;
        public void SetParams(ClapBehaviour.ProxySetings settings)
        {
            // this.stiffness = stiffness;
            //this.damping = damping;
            //this.maxDrive = maxDrive;

            linearDrive = settings.linearSpring.GetJointDriveEquivilent();
            angularDrive = settings.angularSpring.GetJointDriveEquivilent();
            slerpDrive = angularDrive;

            configurableJoint.xDrive = linearDrive;
            configurableJoint.yDrive = linearDrive;
            configurableJoint.zDrive = linearDrive;

            configurableJoint.angularXDrive = angularDrive;
            configurableJoint.angularYZDrive = angularDrive;


            configurableJoint.slerpDrive = slerpDrive;

            configurableJoint.breakForce = maxDrive;

            SetVisibility(settings.showProxyMesh);
        }


        public void ClearParams()
        {
            JointDrive drive = new JointDrive();

            configurableJoint.xDrive = drive;
            configurableJoint.yDrive = drive;
            configurableJoint.zDrive = drive;

            configurableJoint.angularXDrive = drive;
            configurableJoint.angularYZDrive = drive;

            configurableJoint.slerpDrive = drive;

            configurableJoint.breakForce = 100000f;
        }


        public void EnableSpring(bool b)
        {
            //Give it the real rigidbody
            jointToggler.SetConnectedBody(realRb);

            //the joint toggler handles the enabling and disabling of the spring joint
            jointToggler.enabled = b;

        }

        public void SetMyMesh(MyMesh mesh)
        {
            myMesh.CopyMesh(mesh);
        }

        public void SetVisibility(bool visible)
        {
            myMesh.SetVisibility(visible);
        }


        /// <summary>
        /// Ignora all collisions between the interaction object and any collider in this proxy
        /// All with all
        /// </summary>
        /// <param name="io"></param>
        public void IgnoreCollisionsWithObject(InteractionObject io)
        {


            Collider proxyCollider = GetComponent<Collider>();
            Collider ioCollider = io.GetComponent<Collider>();
            Collider[] proxyChildrenColliders = GetComponentsInChildren<Collider>();
            Collider[] ioChildrenColliders = GetComponentsInChildren<Collider>();


            //Ignore collisions with colliders on this gameobject and the io gameobject
            if (ioCollider && proxyCollider)
            {
                Physics.IgnoreCollision(ioCollider, proxyCollider, true);
            }

            //And ignore collisions with any collider in children gameobjects of this.
            foreach (Collider col in proxyChildrenColliders)
            {
                if (ioCollider)
                {
                    Physics.IgnoreCollision(ioCollider, col, true);
                }
            }

            //and Ignore collisions between children colliders
            foreach (Collider IO_col in ioChildrenColliders)
            {

                //ignore the main proxy GO
                if (proxyCollider)
                {
                    Physics.IgnoreCollision(IO_col, proxyCollider, true);
                }

                //each one of the proxyObject children
                foreach (Collider proxyCol in proxyChildrenColliders)
                {
                    Physics.IgnoreCollision(IO_col, proxyCol, true);
                }
            }


        }

        private void Reset()
        {
            configurableJoint = GetComponent<ConfigurableJoint>();
            jointToggler = GetComponent<JointToggler>();
            myMesh = GetComponent<MyMesh>();
        }

        /*
        public void SetConfigurableJointConstraints(InteractionObject.ConfigJointConstraints constraints)
        {
            configurableJoint.xMotion = constraints.PosXYZRotXYZ[0];
            configurableJoint.yMotion = constraints.PosXYZRotXYZ[1];
            configurableJoint.zMotion = constraints.PosXYZRotXYZ[2];
            configurableJoint.angularXMotion = constraints.PosXYZRotXYZ[3];
            configurableJoint.angularYMotion = constraints.PosXYZRotXYZ[4];
            configurableJoint.angularZMotion = constraints.PosXYZRotXYZ[5];
        }
        */

    }
}