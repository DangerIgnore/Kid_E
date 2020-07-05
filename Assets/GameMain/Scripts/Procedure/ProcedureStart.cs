/*
 * 注释：开始认识数字模块
 */

using GameFramework.Procedure;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

public class ProcedureStart : ProcedureBase
{
    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

/*#if NO_GUIDE
        UIRoot.GetInstance().ShowExperimentalContentUI();
#endif*/
    }

    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);

        GameEntry.Entity.HideAllLoadedEntities();
        GameEntry.Entity.HideAllLoadingEntities();
    }
}
