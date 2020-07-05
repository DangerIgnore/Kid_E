using GameFramework.Procedure;
using GameFramework.Resource;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
/*
 * 注释：项目开始引导流程
 */
public class ProcedureBoot : ProcedureBase
{
    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

        
        ChangeState<ProcedureLoadDataModel>(procedureOwner);
        
       
    }
    protected override void OnDestroy(ProcedureOwner procedureOwner)
    {
        base.OnDestroy(procedureOwner);
    }
}
