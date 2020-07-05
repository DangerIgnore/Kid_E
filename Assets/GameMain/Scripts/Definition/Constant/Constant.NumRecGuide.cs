using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Constant
{
    public static class NumRecGuide
    {
        /// <summary>
        /// 认识数字事件类型
        /// </summary>
        public enum NumRecGuideType
        {
            ParamNone = 0,
        }
        /// <summary>
        /// 数字认知模块引导 正确索引
        /// </summary>
        public enum NumRecGuideCorrectionType
        {
            None = 0,

            StartMusic = 1, 
            InteractGuide = 2,
            UIBtnGuide = 3, 
            UIDescripte = 4,


        }
    }
}
