using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using static Constant.NumRecGuide;

/*
 * 注释：引导事件处理类
 */
public class MusicPlay : MonoBehaviourSingle<MusicPlay>
{
    private int m_StepId = -1;
    private bool m_IsMusicPlayOver = false;
    [HideInInspector]
    public bool m_IsHandShow = false;
    [HideInInspector]
    public bool m_IsUIShow = false;
    [HideInInspector]
    public bool m_IsclickMapBtn = false;
    [HideInInspector]
    public bool m_IsMoved = false;

    [HideInInspector]
    public int MusicSerialId = -1; //音乐加载后的序列编号
    private List<object> m_LoadedAssets = new List<object>();//需要操作流程控制的实体列表。

    /// <summary>
    /// 自身的初始化操作
    /// </summary>
    public void OnInitial()
    {
        GameEntry.Event.Subscribe(PlaySoundSuccessEventArgs.EventId, OnSoundPlaySuccess);
        GameEntry.Event.Subscribe(PlaySoundFailureEventArgs.EventId, OnSoundPlayFailure);

        int tempStepId = -1;
        tempStepId = NumRecGuideManager.GetInstance().GetCurStep();
        m_StepId = tempStepId;
        if (tempStepId != -1)
        {
            //直接播放了第一段音频，现在确认之前的stepid的success是什么时候呗设定为true的
            NumRecGuideManager.GetInstance().ShowStepDescriptionAndPlayMusic(tempStepId);
        }
    }

    private void OnSoundPlayFailure(object sender, GameEventArgs e)
    {
        Debug.Log("Music failure");
        PlaySoundFailureEventArgs se = e as PlaySoundFailureEventArgs;
        if (se == null)
        {
            return;
        }
    }

    public void OnClose()
    {
        m_IsMusicPlayOver = false;

        if (MusicSerialId != -1)
        {
            GameEntry.Sound.StopSound(MusicSerialId);
            MusicSerialId = -1;
        }
        if (GameEntry.Event.Check(PlaySoundSuccessEventArgs.EventId, OnSoundPlaySuccess))
        {
            GameEntry.Event.Unsubscribe(PlaySoundSuccessEventArgs.EventId, OnSoundPlaySuccess);
        }

        if (GameEntry.Event.Check(PlaySoundFailureEventArgs.EventId, OnSoundPlayFailure))
        {
            GameEntry.Event.Unsubscribe(PlaySoundFailureEventArgs.EventId, OnSoundPlayFailure);
        }
    }

    public override void Clear()
    {
        base.Clear();

        if (m_LoadedAssets != null && m_LoadedAssets.Count > 0)
        {
            for (int i = 0; i < m_LoadedAssets.Count; i++)
            {
                GameEntry.Resource.UnloadAsset(m_LoadedAssets[i]);
            }

            m_LoadedAssets.Clear();
        }
    }

    private void OnSoundPlaySuccess(object sender, GameEventArgs e)
    {
        PlaySoundSuccessEventArgs se = e as PlaySoundSuccessEventArgs;
        if (se == null)
        {
            return;
        }

        if (se.SoundAgent.SoundGroup.Name == "Element")
        {
            MusicSerialId = se.SoundAgent.SerialId;
            float time = se.SoundAgent.Length + 1.5f;

            int stepId = NumRecGuideManager.GetInstance().GetCurStep();

            if (stepId != -1)
            {
                //做气泡的信息填充
            }
            StartCoroutine("MusicPlayOver", time);
        }
    }

    private IEnumerator MusicPlayOver(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        int stepId = NumRecGuideManager.GetInstance().GetCurStep();

        m_IsMusicPlayOver = true;
        IsCurrentStepOver();
    }

    public void IsCurrentStepOver()
    {
        int stepId = NumRecGuideManager.GetInstance().GetCurStep();
        if (stepId == 1)
        {
            PushEventNow();
        }
        else if (stepId == 2)
        {
            if (m_IsMusicPlayOver && m_IsHandShow)//在这里添加其他条件：是否显示UI挂件或者按钮点击显示地图
            {
                PushEventNow();
            }
        }
        else if (stepId == 3)
        {
            if ((m_IsMusicPlayOver && m_IsUIShow))
            {
                PushEventNow();
            }
        }
        else if (stepId == 4)
        {
            if (m_IsMusicPlayOver)
            {
                PushEventNow();
            }
        }
        else if (stepId == 5)
        {
            if (m_IsMusicPlayOver && m_IsclickMapBtn)
            {
                PushEventNow();
            }
        }
        else if (stepId == 6)
        {
            if (m_IsMusicPlayOver && m_IsMoved)
            {
                PushEventNow();
            }
        }
    }
    //先更新引导状态表，再触发下一次音乐播放
    private void PushEventNow()
    {
        m_StepId = NumRecGuideManager.GetInstance().GetCurStep();
        StepEventArgs e = new StepEventArgs();
        e.Sender = null;

        if (m_StepId == 1)
        {
            e.StepCorrectId = NumRecGuideCorrectionType.StartMusic;
            StepEvent.PushEvent((int)NumRecGuideType.ParamNone, e);
            m_IsMusicPlayOver = false;
            m_IsHandShow = false;

            PlayMusicByCurStep();
        }
        else if (m_StepId == 2)
        {
            e.StepCorrectId = NumRecGuideCorrectionType.InteractGuide;
            StepEvent.PushEvent((int)NumRecGuideType.ParamNone, e);
            m_IsMusicPlayOver = false;
            m_IsHandShow = false;

            PlayMusicByCurStep();
        }
        else if (m_StepId == 3)
        {
            e.StepCorrectId = NumRecGuideCorrectionType.UIBtnGuide;
            StepEvent.PushEvent((int)NumRecGuideType.ParamNone, e);
            m_IsMusicPlayOver = false;

            PlayMusicByCurStep();
        }
        else if (m_StepId == 4)
        {
            //通过事件更新状态表 
            e.StepCorrectId = NumRecGuideCorrectionType.UIDescripte;
            StepEvent.PushEvent((int)NumRecGuideType.ParamNone, e);
            m_IsMusicPlayOver = false;
            //根据最新状态表播放音乐
            PlayMusicByCurStep();
        }
        else if (m_StepId == 5)
        {
            //通过事件更新状态表 
            e.StepCorrectId = NumRecGuideCorrectionType.OpenMap;
            StepEvent.PushEvent((int)NumRecGuideType.ParamNone, e);
            m_IsMusicPlayOver = false;
            m_IsclickMapBtn = false;
            //根据最新状态表播放音乐
            PlayMusicByCurStep();
        }
        else if (m_StepId == 6)
        {
            //通过事件更新状态表 
            e.StepCorrectId = NumRecGuideCorrectionType.MoveGuide;
            StepEvent.PushEvent((int)NumRecGuideType.ParamNone, e);
            m_IsMusicPlayOver = false;
            m_IsMoved = false;
            //根据最新状态表播放音乐
            PlayMusicByCurStep();
        }
    }

    private void PlayMusicByCurStep()
    {
        //播放下一项音乐
        m_StepId = NumRecGuideManager.GetInstance().GetCurStep();
        if (m_StepId != -1)
        {
            NumRecGuideManager.GetInstance().ShowStepDescriptionAndPlayMusic(m_StepId);
        }
    }
}
