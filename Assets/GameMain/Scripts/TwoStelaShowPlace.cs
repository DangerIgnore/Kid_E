using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoStelaShowPlace : MonoBehaviour
{
    // Start is called before the first frame update
    private HighlightAuto highLightCtrl;
    void Start()
    {
        highLightCtrl = GetComponentInChildren<HighlightAuto>();
    }

    private void OnTriggerEnter(Collider other)
    {
        print("OnTriggerEnter");
        if(other.name == "NumTwo")
        {
            highLightCtrl.EdgeLightingConstanting(true, Color.green);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        print("OnTriggerEnter");
        if (collision.gameObject.name == "NumTwo")
        {
            highLightCtrl.EdgeLightingConstanting(true, Color.green);
        }
    }
}
