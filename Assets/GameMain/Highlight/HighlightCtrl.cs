using UnityEngine;


/// <summary>
/// 高光控制类
/// </summary>
public class HighlightCtrl : HighlightableObject
{
    /// <summary>
    /// 是否打开高光
    /// </summary>
    public bool IsOnOff { get; set; } = true;
    /// <summary>
    /// 是否亮一下
    /// </summary>
    private bool Oncing { get; set; } = false;
    /// <summary>
    /// 是否常亮
    /// </summary>
    private bool Constanting { get; set; } = false;
    /// <summary>
    /// 是否闪亮
    /// </summary>
    private bool Flashing { get; set; } = false;
    /// <summary>
    /// 亮一下的颜色
    /// </summary>
    private Color OnceColor { get; set; } = Color.clear;
    /// <summary>
    /// 常亮颜色
    /// </summary>
    private Color ConstantColor { get; set; } = Color.clear;
    /// <summary>
    /// 闪亮起始颜色
    /// </summary>
    private Color FlashColorBegin { get; set; } = Color.clear;
    /// <summary>
    /// 闪亮结束颜色
    /// </summary>
    private Color FlashColorEnd { get; set; } = Color.clear;
    /// <summary>
    /// 闪亮频率
    /// </summary>
    private float FlashingFreq { get; set; } = 0.3f;


    /// <summary>
    /// 描绘边缘高光(一次)
    /// </summary>
    /// <param name="isOpen">是否打开</param>
    /// <param name="color">颜色</param>
    public void EdgeLightingOnce(bool isOpen, Color color)
    {
        Oncing = isOpen;
        OnceColor = color;
        //LightingCtrl();
    }

    /// <summary>
    /// 描绘边缘高光(常亮)
    /// </summary>
    /// <param name="isOpen">是否打开</param>
    /// <param name="color">颜色</param>
    public void EdgeLightingConstanting(bool isOpen, Color color)
    {
        Constanting = isOpen;
        ConstantColor = color;
        LightingCtrl();
    }

    /// <summary>
    /// 描绘边缘高光(闪亮)
    /// </summary>
    /// <param name="isOpen">是否打开</param>
    /// <param name="begin">起始颜色</param>
    /// <param name="end">结束颜色</param>
    public void EdgeLightingFlashing(bool isOpen, Color begin, Color end)
    {
        Flashing = isOpen;
        FlashColorBegin = begin;
        FlashColorEnd = end;
        LightingCtrl();
    }

    /// <summary>
    /// 描绘边缘高光(闪亮)
    /// </summary>
    /// <param name="isOpen">是否打开</param>
    /// <param name="begin">起始颜色</param>
    /// <param name="end">结束颜色</param>
    /// <param name="freq">闪光频率</param>
    public void EdgeLightingFlashing(bool isOpen, Color begin, Color end, float freq)
    {
        Flashing = isOpen;
        FlashColorBegin = begin;
        FlashColorEnd = end;
        FlashingFreq = freq;
        LightingCtrl();
    }

    /// <summary>
    /// 重置高光参数
    /// </summary>
    public void ResetHighlight()
    {
        IsOnOff = true;
        Oncing = false;
        Constanting = false;
        Flashing = false;
        OnceColor = Color.clear;
        ConstantColor = Color.clear;
        FlashColorBegin = Color.clear;
        FlashColorEnd = Color.clear;
        FlashingFreq = 0.3f;
    }

    private void LightingCtrl()
    {
        // 如果开关是关,则不高亮
        if (!IsOnOff)
        {
            Off();
            //ReinitMaterials();
        }
        // 否则先亮一下,后闪亮,最后常亮
        else
        {
            if (Flashing && !IsFlashing)
            {
                ConstantOff();
                FlashingOn(FlashColorBegin, FlashColorEnd, FlashingFreq);
            }
            if (!Flashing && IsFlashing)
            {
                ConstantOff();
                FlashingOff();
            }
            if (Constanting && !IsConstantly)
            {
                FlashingOff();
                ConstantOn(ConstantColor);
            }
            if (!Constanting && IsConstantly)
            {
                FlashingOff();
                ConstantOff();
            }
            if (Oncing && !IsOnce)
            {
                On(OnceColor);
            }
        }
    }
}
