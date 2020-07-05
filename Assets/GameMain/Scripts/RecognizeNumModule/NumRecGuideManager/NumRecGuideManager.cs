using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constant.NumRecGuide;

public class NumRecGuideManager : QSingleton<NumRecGuideManager>
{
    public List<NumRecGuideStepInfo> CurStepSet { get; private set; }//当前步骤信息
    public bool mIsStepOver = false;
    public bool mIsInBeginMode = false;//开始标志位

    public void OnInitial()
    {
        mIsInBeginMode = true;
        mIsStepOver = false;
        StepEvent.RegEvent((int)NumRecGuideCorrectionType.None, OnTaskStep);
        LoadCurTaskStepTable();
        MusicPlay.GetInstance().OnInitial();
    }

    /// <summary>
    /// 退出任务系统函数,注销消息监听
    /// </summary> 
    public void OnClose()
    {
        mIsStepOver = true;
        mIsInBeginMode = false;
        StepEvent.UnRegEvent((int)NumRecGuideType.ParamNone, OnTaskStep);
        MusicPlay.GetInstance().OnClose();
    }

    private void LoadCurTaskStepTable()
    {
        List<NumRecGuideInfo> NumRecGuideInfoData = PropsDataManager.NumRecGuideInfoData.GetAllNumRecGuideData();
        if (NumRecGuideInfoData == null || NumRecGuideInfoData.Count <= 0)//没有或者长度小于零
        {
            return;
        }
        CurStepSet = new List<NumRecGuideStepInfo>();//在这里将引导信息全部存入
        int lastReadStepId = -1;

        foreach (NumRecGuideInfo item in NumRecGuideInfoData)
        {
            NumRecGuideStepInfo stepInfo = new NumRecGuideStepInfo();
            if (item.StepId != lastReadStepId)
            {
                stepInfo.StepId = item.StepId;
                stepInfo.StepDescription = item.StepDescription;
                stepInfo.StepMusic = item.StepMusic;
                stepInfo.StepCount = item.StepCount;
                stepInfo.MusicTime = item.MusicTime;
                stepInfo.BubbleTip = item.BubbleTip;
                lastReadStepId = item.StepId;

                if (stepInfo.CurStepId == -1)
                {
                    stepInfo.CurStepId = item.StepId;
                }

                CurStepSet.Add(stepInfo);
            }
        }
    }

    private void OnTaskStep(int eventTypeId, StepEventArgs e)
    {
        if (e.StepCorrectId != NumRecGuideCorrectionType.None)
        {
            UpdateNumRecGuideStateTable((int)e.StepCorrectId, eventTypeId, e);
        }
    }

    /// <summary>
    /// 获得当前步骤ID，之前步骤都是IsSuccess为true，代表已经顺利完成的，这里的当前步骤id是指正确的下一个步骤id
    /// </summary>
    /// <returns>任务Id</returns>
    public int GetCurStep()
    {
        int stepId = -1;
        if (CurStepSet != null)
        {
            foreach (NumRecGuideStepInfo item in CurStepSet)
            {
                if (!item.IsSuccess)
                {
                    stepId = item.StepId;
                    break;
                }
            }
        }
        return stepId;
    }
    /// <summary>
    /// 认识数字引导中 更新步骤状态表
    /// </summary>
    /// <param name="stepCorrectId">事件所处的步骤Id</param>
    /// <param name="eventTypeId"></param>
    /// <param name="e"></param>
    private void UpdateNumRecGuideStateTable(int stepCorrectId, int eventTypeId, StepEventArgs e)
    {
        //Debug.Log("更新认识数字引导步骤");
        int currentStepId = 0;
        if(CurStepSet == null)
        {
            return;
        }
        foreach(NumRecGuideStepInfo item in CurStepSet)
        {
            if (item.IsSuccess)
            {
                continue;
            }
            //与正确的事件索引不同，直接退出，更新过程直接停止。(异常)
            if(item.CurStepId!= stepCorrectId)
            {
                break;
            }
            if(item.CurStepId == stepCorrectId)
            {
                currentStepId = stepCorrectId;
                if (item.CurStepCount < item.StepCount)
                {
                    if (e.StepCorrectId!=NumRecGuideCorrectionType.None)
                    {
                        item.CurStepCount++;
                    }
                }
                if (item.CurStepCount >= item.StepCount)
                {
                    item.CurStepCount = item.StepCount;
                    item.IsSuccess = true;//完成运行次数要求
                }
            }
            break;
        }
        UpdateCurStepPointer(currentStepId);
    }
    private void UpdateCurStepPointer(int currentStepId)
    {
        if(CurStepSet == null)
        {
            return;
        }
        foreach(NumRecGuideStepInfo item in CurStepSet)
        {
            if (item.IsSuccess)
            {
                //如果没有更新状态表，当前步骤指针不做任何操作，如果更新了状态表，当前状态指针则会指向下一个未完成的状态。
                if(item.StepId == currentStepId && item.IsSuccess)
                {
                    int nextStepId = CurStepSet.IndexOf(item) + 1;
                    if (nextStepId < CurStepSet.Count)
                    {
                        item.StepId = CurStepSet[nextStepId].StepId;
                        //气泡文字内容填充(暂时不做)
                    }
                }
            }
        }
    }

    public void ShowStepDescriptionAndPlayMusic(int curStepId)
    {
        if (!mIsInBeginMode)
        {
            return;
        }

        if (curStepId != -1)
        {
            Dictionary<string, string> pairs = PropsDataManager.NumRecGuideInfoData.GetStepDescriptionAndMusicByStepID(curStepId);//得到步骤说明与音频
            if(pairs!= null)
            {
                foreach(KeyValuePair<string,string> items in pairs)
                {
                    string assetName = AssetUtility.GetMusicAsset(items.Value);
                    GameEntry.Sound.PlaySound(assetName, "Element");
                    break;
                }
            }
        }
    }


    public override void Clear()
    {

    }

    public override void OnReConnect()
    {

    }

    public override void OnUpdate(float deltaTime)
    {

    }

    protected override void OnInit()
    {
        
    }
}
