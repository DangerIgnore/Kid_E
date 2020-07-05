using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraspState : MonoBehaviour
{
    private Rigidbody rb;
    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
    }
    public void changeIK(bool state)
    {
        rb.isKinematic = state;
    }
}
