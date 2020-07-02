using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clap
{
    [RequireComponent(typeof(MyMesh))]
    public class ClapBone : MonoBehaviour
    {
        [SerializeField]
        MyMesh mm;

        public void SetMaterial(Material m)
        {
            mm.SetMaterial(m);
        }

        public void SetVisibility(bool b)
        {
            mm.SetVisibility(b);
        }

        public void SetActive(bool b)
        {
            gameObject.SetActive(b);
        }


        void Setup()
        {
            if (mm == null)
            {
                mm = GetComponent<MyMesh>();
            }
        }

        //Called by unity when adding the component or calling reset function.
        void Reset()
        {
            Setup();
        }

    }
}