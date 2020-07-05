using GameFramework;
using GameFramework.DataTable;
using GameFramework.Event;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
/*
 * 注释：加载各类数据表流程
 */
public class ProcedureLoadDataModel : ProcedureBase
{
    private bool m_IsComplete = false;//数据表全部加载完成
    private int m_DataTableCount = 0;
    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

        // 订阅加载成功事件
        GameEntry.Event.Subscribe(UnityGameFramework.Runtime.LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);

        // 加载配置表
        m_DataTableCount = 1;

        GameEntry.DataTable.LoadDataTable<DRNumRecGuideInfo>(Constant.DataNodeRootName.NumRecGuideInfoRoot, AssetUtility.GetDataTableAsset(Constant.DataNodeRootName.NumRecGuideInfoRoot, LoadType.Text), LoadType.Text);
    }

    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        if (m_IsComplete)
        {
            ChangeState<ProcedureNumRecGuide>(procedureOwner);
        }
    }

    private void OnLoadDataTableSuccess(object sender, GameEventArgs e)
    {
        // 数据表加载成功事件
        UnityGameFramework.Runtime.LoadDataTableSuccessEventArgs ne = e as UnityGameFramework.Runtime.LoadDataTableSuccessEventArgs;
        if (ne == null)
        {
            return;
        }

        Log.Info("Load data table '{0}' success.", ne.DataTableName);

        m_DataTableCount--;
        if (ne.DataTableName.Equals(Constant.DataNodeRootName.NumRecGuideInfoRoot))
        {
            IDataTable<DRNumRecGuideInfo> dtNumRecGuideInfo = GameEntry.DataTable.GetDataTable<DRNumRecGuideInfo>();

            // 获得所有行
            DRNumRecGuideInfo[] drNumRecGuideInfo = dtNumRecGuideInfo.GetAllDataRows();

            foreach (DRNumRecGuideInfo item in drNumRecGuideInfo)
            {
                PropsDataManager.NumRecGuideInfoData.AddNumRecGuide(item.NumRecGuideInfoSet.StepId, item.NumRecGuideInfoSet);
            }
        }
        if (m_DataTableCount <= 0)
        {
            m_IsComplete = true;
        }
    }
    private void OnLoadDataTableFailure(object sender, GameEventArgs e)
    {
        UnityGameFramework.Runtime.LoadDataTableFailureEventArgs ne = e as UnityGameFramework.Runtime.LoadDataTableFailureEventArgs;

        Log.Error("Load data table '{0}' Failure : " + ne.ErrorMessage, ne.DataTableName);
    }
}
