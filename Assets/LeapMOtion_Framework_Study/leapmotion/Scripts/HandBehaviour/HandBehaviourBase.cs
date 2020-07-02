using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using Leap.Unity;

public enum HandState
{
    Normal,
    Pinch,
    CanGrasp,
}

public class HandBehaviourBase : MonoBehaviour
{
    protected HandState handState = HandState.Normal;
    public HandState HandState
    {
        get { return handState; }
    }
    //左手
    protected Transform leftHand;
    public Transform LeftHand
    {
        get { return leftHand; }
    }

    protected Transform leftHandPalm;
    protected InteractionHand leftHandManager;
    protected bool isLeftHandPinched = false;

    //右手
    protected Transform rightHand;
    public Transform RightHand
    {
        get { return rightHand; }
    }

    protected Transform rightHandPalm;
    protected InteractionHand rightHandManager;
    protected bool isRightHandPinched = false;

    protected Transform dragTarget;
    protected GameObject[] PaintTargetObjs;

    protected void Awake()
    {
        Transform Interaction_Manager = GameObject.Find("Interaction Manager").transform;
        leftHand = Interaction_Manager.GetChild(0);
        rightHand = Interaction_Manager.GetChild(1);
        leftHandManager = leftHand.GetComponent<InteractionHand>();
        rightHandManager = rightHand.GetComponent<InteractionHand>();
        InitPinchedDetector();
    }
    protected void Start()
    {
        
    }
   

    protected Transform leftHandModel = null;
    protected Transform rightHandModel = null;
    #region 初始化DetectorPinch
    /// <summary>
    /// 初始化PinchDetector
    /// </summary>
    protected void InitPinchedDetector()
    {
        HandModelManager handModelManager = FindObjectOfType<HandModelManager>();

        leftHandModel = handModelManager.transform.GetChild(2);
        rightHandModel = handModelManager.transform.GetChild(3);
        PinchDetector leftPinchDetector = leftHandModel.gameObject.AddComponent<PinchDetector>();
        PinchDetector rightPinchDetector = rightHandModel.gameObject.AddComponent<PinchDetector>();
        //左手
        leftPinchDetector.OnActivate = new UnityEngine.Events.UnityEvent();
        leftPinchDetector.OnDeactivate = new UnityEngine.Events.UnityEvent();
        leftPinchDetector.OnActivate.AddListener(() => { SetHandPinched(ref isLeftHandPinched, true); });
        leftPinchDetector.OnDeactivate.AddListener(() => { SetHandPinched(ref isLeftHandPinched, false); });
        //右手
        rightPinchDetector.OnActivate = new UnityEngine.Events.UnityEvent();
        rightPinchDetector.OnDeactivate = new UnityEngine.Events.UnityEvent();
        rightPinchDetector.OnActivate.AddListener(() => { SetHandPinched(ref isRightHandPinched, true); });
        rightPinchDetector.OnDeactivate.AddListener(() => { SetHandPinched(ref isRightHandPinched, false); });
    }
    private void SetHandPinched(ref bool handPinched,bool state)
    {
        handPinched = state;
    }
    #endregion

    #region 控制抓取
    protected void ChangeLeftGraspedState()
    {
        if(isLeftHandPinched)
            leftHandManager.graspingEnabled = true;
        else
            leftHandManager.graspingEnabled = false;
    }
    protected void ChangeRightGraspedState()
    {
        if (isRightHandPinched)
        {
            rightHandManager.graspingEnabled = true;
            Debug.Log("Right true!");
        }
        else
        {
            Debug.Log("Right false!");
            rightHandManager.graspingEnabled = false;
        }
    }
    #endregion
    #region 控制物体
    protected void LeftHandPinchedDrag(float speed)
    {
        if(isLeftHandPinched && !isRightHandPinched)
        {
            //dragTarget.Rotate(Vector3.down, leftHandManager.velocity.x * Time.deltaTime*speed);
        }
    }
    private float oldDistance = 0f;
    protected void ChangeScale(float minSize = 0.5f,float maxSize=1.5f)
    {
        float baseDistance = 0.2f;
        //mVector3 originScale = dragTarget.localScale;
        if (!isLeftHandPinched || !isRightHandPinched) return;
        if (isRightHandPinched && isLeftHandPinched)
        {
            float newDistance = Mathf.Abs(rightHandManager.position.ToVector().x - leftHandManager.position.ToVector().x);
            
            print("baseDistance:" + baseDistance);
            float scale = 0;
            //防止小范围内的抖动
            if (Mathf.Abs(newDistance - oldDistance) > 0.001f)
            {
                float finalScaleFactor = (newDistance);
                print("finalScaleFactor" + finalScaleFactor);

                //dragTarget.localScale = finalScaleFactor * originScale;
                oldDistance = newDistance;
            }
        }
            

    }
    #endregion
}
