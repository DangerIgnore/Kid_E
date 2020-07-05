using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;
 
public class GestureTest : MonoBehaviour
{

    public float DisFromHandAndHead;
    private Transform HandFollow;

    public Transform player;
    /// <summary>
    /// 方向 
    /// </summary>
    public Transform dic;
    public Transform leftShoulderPos;

    /// <summary>
    /// 速度
    /// </summary>
    public float speed;
    public float showdelat;
    public float ShowDelta;
    public HandModelBase leftHandModel;
    public Hand leftHand;
    [Tooltip("Velocity (m/s) move toward ")]
    protected float deltaVelocity = 0.7f;
    //这里传进来你要打开的手指 紧握手指 {} 传一个手指{Finger.FingerType.TYPE_RING}...以此类推，当传进5个值得时候代表 手张开，当传进0个值的时候代表 握手
    Finger.FingerType[] OK = { Finger.FingerType.TYPE_THUMB };
    Finger.FingerType[] arr1 = { Finger.FingerType.TYPE_INDEX };
    Finger.FingerType[] Yeah = { Finger.FingerType.TYPE_INDEX, Finger.FingerType.TYPE_MIDDLE };
    Finger.FingerType[] FullOpen = { Finger.FingerType.TYPE_INDEX, Finger.FingerType.TYPE_MIDDLE, 
        Finger.FingerType.TYPE_PINKY, Finger.FingerType.TYPE_RING, Finger.FingerType.TYPE_THUMB };


    Brush brush;
    private void Awake()
    {
        brush = GameObject.Find("SpongeBrush").GetComponent<Brush>();
        HandFollow = GameObject.Find("Leap Rig").transform;
    }
    private void Start()
    {
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        if (!leftHandModel.IsTracked) {
            return;
        } 
        leftHand = leftHandModel.GetLeapHand();
        DisFromHandAndHead = (leftHand.PalmPosition.ToVector3() - leftShoulderPos.position).magnitude;
        showdelat = ShowDelta;
        if (brush == null)
        {
            Debug.Log("No scripts");
            return;
        }
        /*if(CheckFingerOpenToHand(leftHand, arr1))
        {
            brush.paintState = Brush.PaintState.white;
            Debug.Log("PaintState.white");
        }
        if (CheckFingerOpenToHand(leftHand, Yeah))
        {
            brush.paintState = Brush.PaintState.clear;
            Debug.Log("PaintState.clear");
        }*/
        
        //if (IsMoveLeft(leftHand))
        //{
        //    print("左手向左滑动");
        //}
        //if (IsMoveRight(leftHand))
        //{
        //    print("左手向右滑动");
        //}
        //if (IsMoveUp(leftHand))
        //{
        //    print("左手向上滑动");
        //}
        //if (IsMoveDown(leftHand))
        //{
        //    print("左手向下滑动");
        //}

        //if (IsCloseHand(leftHand))
        //{
        //    print("握拳");
        //}
        //if (IsOpenFullHand(leftHand))
        //{
        //    print("张手");
        //}
        //if (CheckFingerCloseToHand(leftHand))
        //{
        //    print("四指指向掌心");
        //}

        if (CheckFingerOpenToHand(leftHand, OK) && (DisFromHandAndHead > 0.4f)&& isSameDirection(dic.forward, leftHand.Direction))
        {
            player.Translate(dic.forward * Time.deltaTime * speed* (DisFromHandAndHead - 0.4f) /(0.8f- DisFromHandAndHead));
            HandFollow.transform.position = player.transform.position;
            HandFollow.transform.rotation = player.transform.rotation;
            //Debug.Log("(" + player.position.x + "," + player.position.y + "," + player.position.x + ")");
        }
    }
    /// <summary>
    /// 这个方法用来扩展哪几个手指打开，这里传进来你要判断是否打开的手指 紧握手指 {} 传一个手指{Finger.FingerType.TYPE_RING}...
    /// 以此类推，当传进5个值得时候代表 手张开，当传进0个值的时候代表 握手
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="arr"></param>
    /// <returns></returns>
    public bool CheckFingerOpenToHand(Hand hand, Finger.FingerType[] fingerTypesArr, float deltaCloseFinger = 0.08f)
    {
        List<Finger> listOfFingers = hand.Fingers;
        float count = 0;
        // 遍历5个手指
        for (int f = 0; f < listOfFingers.Count; f++)
        {
            Finger finger = listOfFingers[f];

            ShowDelta = (finger.TipPosition - hand.PalmPosition).Magnitude;

            // 判读每个手指的指尖位置和掌心位置的长度是不是小于某个值，以判断手指是否贴着掌心
            if ((finger.TipPosition - hand.PalmPosition).Magnitude < deltaCloseFinger)
            {
                // 如果传进来的数组长度是0，有一个手指那么 count + 1，continue 跳出，不执行下面数组长度不是0 的逻辑
                if (fingerTypesArr.Length == 0)
                {
                    count++;
                    continue;
                }
                // 传进来的数组长度不是 0，
                for (int i = 0; i < fingerTypesArr.Length; i++)
                {
                    // 假如本例子传进来的是食指和中指，逻辑走到这里，如果你的食指是紧握的，下面会判断这个手指是不是食指，返回 false
                    if (finger.Type == fingerTypesArr[i])
                    {
                        return false;
                    }
                    else
                    {
                        count++;
                    }
                }

            }
        }
        if (fingerTypesArr.Length == 0)
        {
            return count == 5;
        }
        // 这里除以length 是因为上面数组在每次 for 循环 count ++ 会执行 length 次
        return (count / fingerTypesArr.Length == 5 - fingerTypesArr.Length);
    }

    /// <summary>
    /// 判断是否抓取
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    bool isGrabHand(Hand hand)
    {
        return hand.GrabStrength > 0.8f;
    }

    /// <summary>
    /// 判断是不是握拳
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    bool IsCloseHand(Hand hand)
    {
        List<Finger> listOfFingers = hand.Fingers;
        int count = 0;
        for (int f = 0; f < listOfFingers.Count; f++)
        {
            Finger finger = listOfFingers[f];
            if ((finger.TipPosition - hand.PalmPosition).Magnitude < 0.05f)
            {
                count++;
            }
        }
        return (count == 4);
    }

    /// <summary>
    /// 判断手指是否全张开
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    public bool IsLeftHandOpenFullHand()
    {
        return leftHand.GrabStrength == 0;
    }
    public bool IsLeftHandOpenFullIT()
    {
       return CheckFingerOpenToHand(leftHand, FullOpen,0.08f);
        
    }
    /// <summary>
    /// 手滑向左边
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    protected bool IsMoveLeft(Hand hand)   // 手划向左边
    {
        //x轴移动的速度   deltaVelocity = 0.7f   
        return hand.PalmVelocity.x < -deltaVelocity;
    }

    /// <summary>
    /// 手滑向右边
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    protected bool IsMoveRight(Hand hand)
    {
        return hand.PalmVelocity.x > deltaVelocity;
    }

    /// <summary>
    /// 手滑向上边
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    protected bool IsMoveUp(Hand hand)
    {
        return hand.PalmVelocity.y > deltaVelocity;
    }

    /// <summary>
    /// 手滑向下边
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    protected bool IsMoveDown(Hand hand)
    {
        return hand.PalmVelocity.y < -deltaVelocity;
    }


    /// <summary>
    /// 判断四指是否靠向掌心
    /// </summary>
    /// <param name="hand"></param>
    /// <returns></returns>
    public bool CheckFingerCloseToHand(Hand hand)
    {
        List<Finger> listOfFingers = hand.Fingers;
        int count = 0;
        for (int f = 0; f < listOfFingers.Count; f++)
        {
            Finger finger = listOfFingers[f];
            if ((finger.TipPosition - hand.PalmPosition).Magnitude < 0.05f)
            {
                if (finger.Type == Finger.FingerType.TYPE_THUMB)
                {
                    return false;
                }
                else
                {
                    count++;
                }
            }
        }
        return (count == 4);
    }
    /// <summary>
    /// 获得两向量的角度
    /// </summary>
    /// <param name="a">向量1</param>
    /// <param name="b">向量2</param>
    /// <returns></returns>
    protected float angle2LeapVectors(Leap.Vector a,Leap.Vector b)
    {
        //向量转化成角度
        return Vector3.Angle(UnityVectorExtension.ToVector3(a), UnityVectorExtension.ToVector3(b));
    }
    protected float angle2LeapVectors(Vector3 a, Leap.Vector b)
    {
        //向量转化成角度
        return Vector3.Angle(a, UnityVectorExtension.ToVector3(b));
    }
    protected bool isSameDirection(Vector3 a,Vector b)
    {
        float handForwardDegree = 50f;
        //Debug.Log(angle2LeapVectors(a, b));
        return angle2LeapVectors(a, b) < handForwardDegree;
    }
    protected bool isSameDirection(Vector a, Vector b)
    {
        float handForwardDegree = 50f;
        Debug.Log(angle2LeapVectors(a, b));
        return angle2LeapVectors(a, b) < handForwardDegree;
    }
}