
using System;
using UnityEngine;
using static Constant.NumRecGuide;

public class StepEventArgs : EventArgs
{
    public GameObject Sender { get; set; } = null;
    public NumRecGuideCorrectionType StepCorrectId { get; set; } = NumRecGuideCorrectionType.None;// 正确步骤的Id，初始化为none
}
