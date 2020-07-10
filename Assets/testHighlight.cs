using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testHighlight : MonoBehaviour
{
    private HighlightAuto lightctrl;
    // Start is called before the first frame update
    void Start()
    {
        lightctrl = GetComponent<HighlightAuto>();
        lightctrl.EdgeLightingConstanting(true, Color.green);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
