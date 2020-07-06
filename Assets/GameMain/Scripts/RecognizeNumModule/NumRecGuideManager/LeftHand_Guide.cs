using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand_Guide : MonoBehaviourSingle<LeftHand_Guide>
{
    public GameObject mHead;
    [HideInInspector]
    public GameObject mLeftHand = null;
    [HideInInspector]
    public int m_count = 0;
    [HideInInspector]
    public bool mIsLighting = false;//是否激活显示手部

    public void DestorySelf()
    {
        mLeftHand = null;
        mIsLighting = false;
        m_count = 0;
        Destroy(gameObject);
    }

    private void Start()
    {
        mLeftHand = gameObject;
    }

    private void FixedUpdate()
    {
        print(Vector3.Distance(gameObject.transform.position, mHead.transform.position));
        if (mIsLighting && mHead != null && Vector3.Distance(gameObject.transform.position, mHead.transform.position) < 1.25f)
        {
            int stepId = NumRecGuideManager.GetInstance().GetCurStep();
            if (stepId == 2)
            {
                m_count = 1;
                MusicPlay.GetInstance().m_IsHandShow = true;
                MusicPlay.GetInstance().IsCurrentStepOver();
                mIsLighting = false;//关闭显示手部标志
            }
        }
    }
}
