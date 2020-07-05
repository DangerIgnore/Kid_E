using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepEvent
{
    public delegate void StepEventDel(int eventTypeId, StepEventArgs e); //委托句柄
    private static Dictionary<int,List<StepEventDel>> eventDic = new Dictionary<int,List<StepEventDel>>();

    /// <summary>
    /// 注册事件监听
    /// </summary>
    /// <param name="eventTypeId">EventType</param>
    /// <param name="eventDel">StepCorrectType</param>
    public static void RegEvent(int eventTypeId, StepEventDel eventDel)
    {
        if (eventDic != null)
        {
            if (!eventDic.ContainsKey(eventTypeId))
            {
                eventDic.Add(eventTypeId, new List<StepEventDel>());
            }
            eventDic[eventTypeId].Add(eventDel);
        }
    }
    /// <summary>
    /// 解除事件监听
    /// </summary>
    /// <param name="eventTypeId">EventType</param>
    /// <param name="eventDel">StepCorrectType</param>
    public static void UnRegEvent(int eventTypeId, StepEventDel eventDel)
    {
        if (eventDic != null && eventDic.ContainsKey(eventTypeId))
        {
            eventDic[eventTypeId].Remove(eventDel);
        }
    }

    /// <summary>
    /// 抛出事件
    /// </summary>
    /// <param name="eventTypeId">EventType</param>
    public static void PushEvent(int eventTypeId, StepEventArgs e)
    {
        if (eventDic != null && eventDic.ContainsKey(eventTypeId))
        {
            foreach (StepEventDel item in eventDic[eventTypeId])
            {
                item.Invoke(eventTypeId, e);
            }
        }
    }
}
