using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : GameManagerBase
{
    public static MainGameManager Instance = null;
    //初始化手势控制器
    private MainHandBehaviour handBehaviour;
    public MainHandBehaviour HandBehaviour
    {
        get { return handBehaviour; }
    }
    private Transform dragTarget;
    private GameObject[] paintTargetObjs;
    public Transform DragTarget
    {
        get { return dragTarget; }
    }
    public GameObject[] PaintTargetObjs
    {
        get { return paintTargetObjs; }
    }
    new void Awake()
    {
        Instance = this;
        base.Awake();
    }
    protected override void InitGameManager()
    {
        handBehaviour = new GameObject("HandBehaviour").AddComponent<MainHandBehaviour>();
    }
    protected override void InitGameObject()
    {
        
    }
}
