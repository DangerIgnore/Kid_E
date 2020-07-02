using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PaintTools
{
///summary
///控制海绵刷
///summary
public class Controller : MonoBehaviour
{
    public float Speed;
    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MoveForward();
        }
        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveBack();
        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Lrotate();
        }
        if (Input.GetKey(KeyCode.E))
        {
            Rrotate();
        }
    }
    void MoveForward()
    {
        transform.Translate(transform.TransformDirection(transform.forward) * Time.deltaTime * Speed);
    }
    void MoveBack()
    {
        transform.Translate(transform.TransformDirection(transform.forward) * Time.deltaTime * -Speed);
    }
    void MoveLeft()
    {
        transform.Translate(transform.TransformDirection(transform.right) * Time.deltaTime * -Speed);
    }
    void MoveRight()
    {
        transform.Translate(transform.TransformDirection(transform.right) * Time.deltaTime * Speed);
    }
    void Lrotate()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * Speed);
    }
    void Rrotate()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * -Speed);
    }
}
}
