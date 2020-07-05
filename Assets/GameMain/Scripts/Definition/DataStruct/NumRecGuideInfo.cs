/// <summary>
/// 表的数据结构
/// </summary>
public class NumRecGuideInfo
{
    /// <summary>
    /// 步骤编号
    /// </summary>
    public int StepId { get; set; }

    /// <summary>
    /// 步骤描述
    /// </summary>
    public string StepDescription { get; set; }

    /// <summary>
    /// 步骤音效
    /// </summary>
    public string StepMusic { get; set; }

    /// <summary>
    /// 音乐时长
    /// </summary>
    public int MusicTime { get; set; }

    /// <summary>
    /// 步骤执行次数
    /// </summary>
    public int StepCount { get; set; }

    /// <summary>
    /// 气泡提示
    /// </summary>
    public string BubbleTip { get; set; }
}