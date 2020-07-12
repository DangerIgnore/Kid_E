using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoToStelaColl : MonoBehaviour
{
    private bool isFirst = true;
    private bool StartBackHome = false;
    public Material BackHomeMat;
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "StelaTwo")
        {
            collision.gameObject.GetComponentInChildren<HighlightAuto>().EdgeLightingConstanting(true, Color.green);
            StartBackHome = true;
        }
        //print("发生碰撞");
        if (collision.gameObject.tag == "StelaTwo"&& isFirst)
        {
            isFirst = false;
            //print("是二");
            
            collision.gameObject.GetComponentInChildren<HighlightAuto>().EdgeLightingConstanting(true, Color.green);
        }
        else
        {
            if(collision.gameObject.tag == "StelaOne"|| collision.gameObject.tag == "StelaThree")
            {
                //print("不是二");
                StelaMusicCtrl.GetInstance().PlayMusicByName("NotTwoHouse");

            }
            
        }

        if(collision.gameObject.name == "TwoPlace"&& StartBackHome)
        {
            gameObject.transform.position = new Vector3(11, 10, 13);
            collision.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = BackHomeMat;
            StelaMusicCtrl.GetInstance().TwoOver.Invoke();
            //collision.gameObject.GetComponentInChildren<Animation>().Play("TwoBackHome");
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        
    }
}
