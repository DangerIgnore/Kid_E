using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 不同模块的各种配置表管理类,用以替代框架的DataNode;
/// </summary>
public class PropsDataManager
{
    public class NumRecGuideInfoData
    {
        public static Dictionary<int, List<NumRecGuideInfo>> NumRecGuideDic { get; private set; } = new Dictionary<int, List<NumRecGuideInfo>>();
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="conditionId">模块Id</param>
        /// <param name="data">模块场景数据集</param>
        public static void AddNumRecGuide(int conditionId, NumRecGuideInfo data)
        {
            if (NumRecGuideDic != null)
            {
                if (!NumRecGuideDic.ContainsKey(conditionId))
                {
                    NumRecGuideDic.Add(conditionId, new List<NumRecGuideInfo>());
                }
                NumRecGuideDic[conditionId].Add(data);
            }
        }

        /// <summary>
        /// 获得新手引导所有数据
        /// </summary>
        /// <param name="experimentId">模块Id</param>
        /// <returns>模块数据集</returns>
        public static List<NumRecGuideInfo> GetAllNumRecGuideData()
        {
            if (NumRecGuideDic != null)
            {
                List<NumRecGuideInfo> tempList = new List<NumRecGuideInfo>();
                foreach (KeyValuePair<int, List<NumRecGuideInfo>> item in NumRecGuideDic)
                {
                    if (NumRecGuideDic.ContainsKey(item.Key))
                    {
                        foreach (NumRecGuideInfo items in item.Value)
                        {
                            tempList.Add(items);
                        }
                    }
                }
                return tempList;
            }
            return null;
        }

        /// <summary>
        /// 根据Id找到步骤描述和音效
        /// </summary>
        /// <param name="stepId"></param>
        /// <returns>key：步骤描述，value:音效地址</returns>
        public static Dictionary<string, string> GetStepDescriptionAndMusicByStepID(int stepId)
        {
            if (NumRecGuideDic != null)
            {
                Dictionary<string, string> tempDic = new Dictionary<string, string>();
                foreach (KeyValuePair<int, List<NumRecGuideInfo>> item in NumRecGuideDic)
                {
                    if (NumRecGuideDic.ContainsKey(item.Key))
                    {
                        foreach (NumRecGuideInfo items in item.Value)
                        {
                            if (stepId == items.StepId)
                            {
                                tempDic.Add(items.StepDescription, items.StepMusic);
                                break;
                            }
                        }
                    }
                }
                return tempDic;
            }
            return null;
        }

        /// <summary>
        /// 根据ID找到气泡提示
        /// </summary>
        /// <param name="StepId"></param>
        /// <returns></returns>
        public static string GetStepBubbleTipById(int stepId)
        {
            string tempIndex = null;
            if (NumRecGuideDic != null)
            {
                foreach (KeyValuePair<int, List<NumRecGuideInfo>> item in NumRecGuideDic)
                {
                    if (NumRecGuideDic.ContainsKey(item.Key))
                    {
                        foreach (NumRecGuideInfo items in item.Value)
                        {
                            if (stepId == items.StepId)
                            {
                                tempIndex = items.BubbleTip;
                                break;
                            }
                        }
                    }
                }
                return tempIndex;
            }
            return tempIndex;
        }


        /// <summary>
        /// 根据ID找到步骤描述
        /// </summary>
        /// <param name="StepId"></param>
        /// <returns></returns>
        public static string GetStepDescriptionById(int stepId)
        {
            string tempDes = null;
            if (NumRecGuideDic != null)
            {
                foreach (KeyValuePair<int, List<NumRecGuideInfo>> item in NumRecGuideDic)
                {
                    if (NumRecGuideDic.ContainsKey(item.Key))
                    {
                        foreach (NumRecGuideInfo items in item.Value)
                        {
                            if (stepId == items.StepId)
                            {
                                tempDes = items.StepDescription;
                                break;
                            }
                        }
                    }
                }
                return tempDes;
            }
            return tempDes;
        }
    }
}
