using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://gamedev.stackexchange.com/questions/151890/how-to-disable-spring-joint-in-unity-3d
/// </summary>
/// 
namespace Clap
{
    public class JointToggler : MonoBehaviour
    {
        [SerializeField]
        private Joint joint;
        private Rigidbody connectedBody;

        private void Awake()
        {
            joint = joint ? joint : GetComponent<ConfigurableJoint>();
            if (joint) connectedBody = joint.connectedBody;
            else Debug.LogError("No joint found.", this);
        }

        private void OnEnable() { joint.connectedBody = connectedBody; }

        private void OnDisable()
        {
            //joint.connectedBody = null;
            // connectedBody.WakeUp();
        }
        //Called by the proxy in setup.
        public void SetConnectedBody(Rigidbody rb)
        {
            connectedBody = rb;
        }

    }
}