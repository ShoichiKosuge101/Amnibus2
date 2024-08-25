using Cysharp.Threading.Tasks;

namespace Controller.Logic.State
{
    public class PlayerAttackState
        : StateBase
    {
        public PlayerAttackState(in StateManager owner) : base(in owner)
        {
        }

        public override void OnEnter()
        {
            ActionAsync().Forget();
        }

        private async UniTask ActionAsync()
        {
            // 攻撃処理
            await Owner.MapManager.PlayerController.AttackAsync();
            
            // 敵のターンに移動
            Owner.ChangeState(Owner.EnemyMoveState);
        }
    }
}