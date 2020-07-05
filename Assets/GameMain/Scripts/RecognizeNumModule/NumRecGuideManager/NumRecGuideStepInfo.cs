/// <summary>
/// 数据表完成状态数据结构
/// </summary>
public class NumRecGuideStepInfo
{
    public int StepId = -1;                     // 步骤Id
    public string StepDescription = "";         // 步骤描述
    public int StepCount = 0;                   // 步骤执行次数
    public string StepMusic = "";               // 步骤描述
    public int MusicTime = 0;                   // 音乐时长
    public string BubbleTip = "";               // 气泡提示

    public bool IsSuccess = false;              // 步骤是否完成
    public int CurStepCount = 0;                // 当前步骤执行次数
    /// <summary>
    /// 当前步骤ID
    /// </summary>
    public int CurStepId = -1;                  // 当前步骤ID
}
