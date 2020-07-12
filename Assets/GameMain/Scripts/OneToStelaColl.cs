using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneToStelaColl : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "StelaOne")
        {
            StelaMusicCtrl.GetInstance().OneOver.Invoke();
        }
        else
        {
            StelaMusicCtrl.GetInstance().PlayMusicByName("NotOneHouse");
        }
    }
}
