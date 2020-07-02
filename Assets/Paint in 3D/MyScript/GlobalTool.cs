using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTool
{
    public static float startTime = 0;
   public static IEnumerator DelayFuc(Action action,float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();
    }
}
