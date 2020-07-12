using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StelaMusicCtrl : MonoBehaviourSingle<StelaMusicCtrl>
{
    [HideInInspector]
    public UnityEvent OneOver;
    [HideInInspector]
    public UnityEvent TwoOver;
    [HideInInspector]
    public UnityEvent ThreeOver;
    private int backHomeTimes = 0;


    private void Start()
    {
        OneOver = new UnityEvent();
        OneOver.AddListener(StelaMusicPlayOne);
        TwoOver = new UnityEvent();
        TwoOver.AddListener(StelaMusicPlayTwo);
        ThreeOver = new UnityEvent();
        ThreeOver.AddListener(StelaMusicPlayThree);
    }

    private void StelaMusicPlayThree()
    {
        if(backHomeTimes == 3)
        {
            PlayMusicByName("AllHome");
            backHomeTimes = 0;
        }
        else
        {
            PlayMusicByName("ThreeHome");
            backHomeTimes++;
        }
    }

    private void StelaMusicPlayTwo()
    {
        if (backHomeTimes == 3)
        {
            PlayMusicByName("AllHome");
            backHomeTimes = 0;
         }
        else
        {
            PlayMusicByName("TwoHome");
            backHomeTimes++;
        }
    }

    private void StelaMusicPlayOne()
    {
        if (backHomeTimes == 3)
        {
            PlayMusicByName("AllHome");
            backHomeTimes = 0;
        }
        else
        {
            PlayMusicByName("OneHome");
            backHomeTimes++;
        }
    }

    public void PlayMusicByName(string Name)
    {
        GameEntry.Sound.StopAllLoadingSounds();
        GameEntry.Sound.StopAllLoadedSounds();
        string soundAsset = AssetUtility.GetMusicAsset(Name);
        GameEntry.Sound.PlaySound(soundAsset, "Element");
    }
}
