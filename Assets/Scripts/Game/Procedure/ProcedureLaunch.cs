using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using VisualProcedure.Runtime;

namespace Game.Procedure
{
    public class ProcedureLaunch : GameProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            InitGame().Forget();
        }

        private async UniTaskVoid InitGame()
        {
            GameProcedure.Current.StartProcedure();
        }
    }
}