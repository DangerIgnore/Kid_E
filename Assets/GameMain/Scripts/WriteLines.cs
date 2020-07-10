using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HighlightInfo
{
    public HighlightAuto highlightAuto = null;
    public bool m_IsActive = false;

}
public class WriteLines : MonoBehaviour
{
    private bool showWriteLine = true;
    public float delay = .5f;
    public List<MeshRenderer> sphereList;
    private int index = 0;
    public Dictionary<int, HighlightInfo> triggerList = null;
    [HideInInspector]
    public bool m_IsGraped = false;
    private bool startPaint = true;
    private bool isPaint = false;
    private bool startClear = false;
    private bool isClear = false;
    private bool isMoveToTela = false;
    public UnityEvent PlayPaintedMusic;
    private bool PaintedPlayed = false;
    public UnityEvent PlayClearedMusic;
    private bool ClearedPlayed = false;
    public UnityEvent PlayMoveToStelaMusic;
    private bool MoveToStelaPlayed = false;



    private void Start()
    {
        triggerList = new Dictionary<int, HighlightInfo>();
        //pt.AddTimeTask(ChangeMat03, delay, PETimeUnit.Second, 0);
        //InvokeRepeating("ChangeMat01", delay, delay);
        //给所有的子节点添加高光组件，并填充是否触发字典里

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject curNode = transform.GetChild(i).gameObject;
            curNode.AddComponent<HighlightableObject>();
            curNode.AddComponent<HighlightAuto>();
            HighlightInfo curNodeHighlight = new HighlightInfo();
            curNodeHighlight.m_IsActive = false;
            curNodeHighlight.highlightAuto = curNode.GetComponent<HighlightAuto>();
            triggerList.Add(i, curNodeHighlight);
        }
        //ActiveWriteLine();
        //StartCoroutine("ChangeMatContour");

    }
   

    IEnumerator ChangeMatContour()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            ChangeMat01();
        }
    }

    private void ChangeMat01()
    {
        if (index == sphereList.Count)
        {
            BlackAllPoint();
            index = 0;
        }
        else
        {
            LightOneButton(index);
            index++;
        }
        
    }
    public void changeGrapeState(bool state)
    {
        m_IsGraped = state;
        //print(m_IsGraped);
    }

    public void ActiveWriteLineAndStart()
    {
        if(triggerList != null&& showWriteLine)
        {
            foreach(KeyValuePair<int, HighlightInfo> item in triggerList)
            {
                item.Value.m_IsActive = true;
            }
        }
        StartCoroutine("ChangeMatContour");
    }
    public void ActiveWriteLine()
    {
        if (triggerList != null&&showWriteLine)
        {
            foreach (KeyValuePair<int, HighlightInfo> item in triggerList)
            {
                item.Value.m_IsActive = true;
            }
        }
    }
    private void FixedUpdate()
    {
        if (paintFinish())
        {
            isPaint = true;
            m_IsGraped = false;//停止检测是否画完
        }
        if(clearFinish())
        {
            //print("ClearFinish");
            isClear = true;
            m_IsGraped = false;//停止检测是否画完
        }
        if (isPaint)
        {
            isPaint = false;
            PlayClearedMusic.Invoke();
            //print("Invoke PlayClearedMusic");
            //isClear = true;
        }
        if (isClear)
        {
            //print("Invoke PlayMoveToStelaMusic");
            PlayMoveToStelaMusic.Invoke();
            isClear = false;
            isMoveToTela = true;
        }
    }
    /// <summary>
    /// 不是抓起来涂的都不算涂色
    /// </summary>
    /// <returns></returns>
    public bool paintFinish()
    {
        if (m_IsGraped&&startPaint)
        {
            foreach(KeyValuePair<int, HighlightInfo> item in triggerList)
            {
                if (item.Value.m_IsActive == true) return false;
            }
            startPaint = false;
            startClear = true;//可以擦除
            return true;
            
        }
        return false;
    }

    public bool clearFinish()
    {
        if (m_IsGraped && startClear)
        {
            foreach (KeyValuePair<int, HighlightInfo> item in triggerList)
            {
                if (item.Value.m_IsActive == true) return false;
            }
            showWriteLine = false;
            return true;
        }
        return false;
    }

    public void DeActiveWriteLine()
    {
        BlackAllPoint();
        if (triggerList != null)
        {
            foreach (KeyValuePair<int, HighlightInfo> item in triggerList)
            {
                item.Value.m_IsActive = false;
            }
        }
        StopCoroutine("ChangeMatContour");
    }
    /// <summary>
    /// 根据Index高亮某个位置
    /// </summary>
    /// <param name="index"></param>
    public void LightOneButton(int index)
    {
        if (triggerList != null)
        {
            HighlightInfo value = null;
            triggerList.TryGetValue(index,out value);
            if (value != null&&value.m_IsActive)
            {
                value.highlightAuto.EdgeLightingConstanting(true, Color.green);
            }
        }
    }

    /// <summary>
    /// 熄灭所有关键点
    /// </summary>
    public void BlackAllPoint()
    {
        if (triggerList != null)
        {
            foreach (KeyValuePair<int,HighlightInfo> item in triggerList)
            {
                item.Value.highlightAuto.EdgeLightingConstanting(false, Color.clear);
            }
        }
    }

    public void PlayMusicByName(string Name)
    {
        GameEntry.Sound.StopAllLoadingSounds();
        GameEntry.Sound.StopAllLoadedSounds();
        string soundAsset = AssetUtility.GetMusicAsset(Name);
        GameEntry.Sound.PlaySound(soundAsset, "Element");
    }
    public void PlayPaintedMusicCallback()
    {
        if (!PaintedPlayed)
        {
            PlayMusicByName("FindNum2");
            PaintedPlayed = true;
        }
    }
    public void PlayClearedMusicCallback()
    {
        if (!ClearedPlayed)
        {
            PlayMusicByName("ClearNum2");
            ClearedPlayed = true;
        }
    }
    public void PlayMoveToStelaMusicCallback()
    {
        if (!MoveToStelaPlayed)
        {
            PlayMusicByName("Move2Stela");
            MoveToStelaPlayed = true;
        }
    }
}
