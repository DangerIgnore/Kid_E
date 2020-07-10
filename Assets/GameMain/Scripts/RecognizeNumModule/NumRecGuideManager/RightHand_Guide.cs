using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand_Guide : MonoBehaviourSingle<RightHand_Guide>
{
    public GameObject mHead;
    [HideInInspector]
    public GameObject mRightHand = null;
    [HideInInspector]
    public int m_count = 0;
    [HideInInspector]
    public bool mIsLighting = false;//是否激活显示手部

    public void DestorySelf()
    {
        mRightHand = null;
        mIsLighting = false;
        m_count = 0;
        Destroy(gameObject);
    }

    private void Start()
    {
        mRightHand = gameObject;
        //mHead = GameObject.Find("Leap Rig").transform.GetChild(0).gameObject;
    }

    private void FixedUpdate()
    {
        //print(Vector3.Distance(mRightHand.transform.position, mHead.transform.position));

        if (mIsLighting && mHead != null && Vector3.Distance(mRightHand.transform.position, mHead.transform.position) < 1.5f)
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
