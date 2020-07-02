using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///summary
///
///summary
public class DrawLineDetector : MonoBehaviour
{
    private RaycastHit hit;
    private Ray ray;
    int curDrawNum;
    int preDrawNum;
    int maxDrawNum;
    private void Start()
    {
        maxDrawNum = this.transform.childCount;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hit)&& hit.collider.gameObject.tag.Equals("DrawLine"))
            {
                //Debug.Log(hit.collider.gameObject.name);
                curDrawNum = int.Parse(hit.collider.gameObject.name);
                if(curDrawNum == preDrawNum || curDrawNum == (preDrawNum + 1))
                {
                    Debug.Log("书写正确连贯，保持");
                }
                
                if (curDrawNum < preDrawNum)
                {
                    Debug.Log("回笔，书写不正确");
                }
                if (curDrawNum == maxDrawNum)
                {
                    preDrawNum = 0;
                }
                else {
                    preDrawNum = curDrawNum;
                }
            }
        }
        
    }
    
}
