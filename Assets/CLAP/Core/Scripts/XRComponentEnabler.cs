using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Turn on all VR specific related setup in the scene depending on what VR device is connected
/// Thanks to https://answers.unity.com/questions/1441796/what-are-some-of-the-values-of-xrxrdevicemodel.html
/// </summary>
public class XRComponentEnabler : MonoBehaviour
{

    
    // Strings may slightly differ, e.g. "Vive. MV" or "Vive MV.", and not just per OS
    const string model_vive = "Vive MV";
    const string model_rift = "Oculus Rift CV1";
    const string model_lenovoExplorer = "Lenovo Explorer";
    const string model_hpWindowsMixedReality = "HP Windows Mixed Reality Headset";
    const string model_samsungOdyssey = "Samsung Windows Mixed Reality 800ZAA";
    const string model_acer = "Acer AH100";

    public enum ActiveVRFamily { Oculus, Vive, Windows };

    [SerializeField] GameObject[] m_noVREnabled,m_viveSpecific, m_oculusSpecific;

    public string detectedHMD = "";
    public ActiveVRFamily activeVRHMD;
    


    /*
     * 
    public Transform leftAimingNub;
    public Transform rightAimingNub;

    public Vector3 oculusTouchOffset;
    public Vector3 ViveOffset;
    public Vector3 WindowsVROffset;
    */


    private void Awake()
    {
        if (XRDevice.isPresent)
        {
            detectedHMD = XRDevice.model;
            Debug.Log("Detected a VR headset. Adjusted the controllers' lasers accordingly for precise aiming. The HMD is a " + detectedHMD);

            if (detectedHMD.ToLower().Contains("vive"))
            {
                // Must be a Vive headset.
                activeVRHMD = ActiveVRFamily.Vive;
                //leftAimingNub.localEulerAngles = ViveOffset;
                //rightAimingNub.localEulerAngles = ViveOffset;
                foreach (GameObject o in m_noVREnabled)
                {
                    o.SetActive(false);
                }
                foreach (GameObject o in m_oculusSpecific)
                {
                    o.SetActive(false);
                }

                foreach (GameObject o in m_viveSpecific)
                {
                    o.SetActive(true);
                }

            }
            else if (detectedHMD.ToLower().Contains("oculus"))
            {
                // Must be an Oculus headset.
                activeVRHMD = ActiveVRFamily.Oculus;
                //leftAimingNub.localEulerAngles = oculusTouchOffset;
                //rightAimingNub.localEulerAngles = oculusTouchOffset;
                foreach (GameObject o in m_noVREnabled)
                {
                    o.SetActive(false);
                }
                foreach (GameObject o in m_viveSpecific)
                {
                    o.SetActive(false);
                }
                foreach (GameObject o in m_oculusSpecific)
                {
                    o.SetActive(true);
                }
            }
            else
            {
                // Must be a Windows VR headset.
                activeVRHMD = ActiveVRFamily.Windows;
                //leftAimingNub.localEulerAngles = WindowsVROffset;
                //rightAimingNub.localEulerAngles = WindowsVROffset;
            }

        }
        else
        {

            foreach (GameObject o in m_oculusSpecific)
            {
                o.SetActive(false);
            }
            foreach (GameObject o in m_viveSpecific)
            {
                o.SetActive(false);
            }
            foreach (GameObject o in m_noVREnabled)
            {
                o.SetActive(true);
            }
        }
    }

}
