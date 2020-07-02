using PaintIn3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///summary
///刷子脚本，当刷子接触可绘制物体，在接触点生成贴花预制体。
///summary
public class Brush : MonoBehaviour
{
    public float density = 20;
    private int delayTimes = 0;
    Vector3 Oripos;//初始位置记录，用于移动监测
    public enum PaintState
    {
        clear,
        white
    }

    public PaintState paintState;

    //private GameObject Prefab { set { prefab = value; } get { return prefab; } }
    private GameObject prefab;

    public float offset = 0.01f;
    public float paintRadius = 0.01f;
    private void Awake()
    {
        paintState = PaintState.white;
        Oripos = this.transform.localPosition;
    }

    public bool IsStatic()
    {
        var d = Vector3.SqrMagnitude(Oripos - transform.localPosition);
        Oripos = transform.localPosition;
        return d < 0.01f;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            paintState = PaintState.clear;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            paintState = PaintState.white;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "PaintTarget")
        {
            Painting(collision);
        }

        //Debug.Log("create!");
    }

    private void Painting(Collision collision)
    {
        Vector3 position = new Vector3();//记录碰撞的中间点位置
        for (int i = 0; i < collision.contactCount; i++)
        {
            position.x += collision.GetContact(i).point.x;
            position.y += collision.GetContact(i).point.y;
            position.z += collision.GetContact(i).point.z;
        }
        position = position / collision.contactCount;
        Vector3 dir = (this.transform.position - position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir, collision.contacts[0].normal);
        if(paintState == PaintState.white)
        {
            prefab = Resources.Load("PaintPoints/Pigment White") as GameObject;
        }
        else
        {
            prefab = Resources.Load("PaintPoints/Pigment Clear") as GameObject;
        }
        prefab.GetComponent<P3dPaintDecal>().Radius = paintRadius;
        GameObject paintPoint = Instantiate(prefab, position+dir*offset, rot) as GameObject;
        paintPoint.GetComponent<Rigidbody>().velocity = -dir * 1;
        //Debug.Log("create!");
    }
    
    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log("stay!");
        //if (IsStatic()) return;
        if (collision.gameObject.tag == "PaintTarget")
        {
            delayTimes++;
            if(delayTimes == density)
            {
                Painting(collision);
                //Debug.Log("stay create!");
                delayTimes = 0;
            }
        }
        //Debug.Log("stay times!");
    }
}
