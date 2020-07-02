using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity.Interaction;

public class MainHandBehaviour : HandBehaviourBase
{
    private GameObject Num2Obj;

    private InteractionBehaviour InteractionB_2;
    new void Awake()
    {
        dragTarget = MainGameManager.Instance.DragTarget;
        PaintTargetObjs = MainGameManager.Instance.PaintTargetObjs;
        
        
        base.Awake();
    }

    new void Start()
    {
        foreach (var TargetObj in PaintTargetObjs)
        {
            if (TargetObj.name == "LittleCrocHI_prefab")
            {
                Num2Obj = TargetObj;
                InteractionB_2 = Num2Obj.GetComponent<InteractionBehaviour>();
            }
        }

        base.Start();
    }
    private void Update()
    {
        //LeftHandPinchedDrag(800);
        //ChangeScale();
        //ChangeLeftGraspedState();
        //ChangeRightGraspedState();
    }
}
