using GameFramework.Procedure;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game.Procedure
{
    public abstract class GameProcedureBase : ProcedureBase
    {
        protected ProcedureOwner owner;
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            owner = procedureOwner;
        }

        public void ChangeProcedure<T>() where T : GameProcedureBase
        {
            ChangeState<T>(owner);
        }
    }
}