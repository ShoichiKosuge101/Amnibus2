using Cysharp.Threading.Tasks;

namespace Controller.Logic.State
{
    public class EnemyAttackState
        : StateBase
    {
        public EnemyAttackState(in StateManager owner) : base(in owner)
        {
        }

        public override void OnEnter()
        {
            ActionAsync().Forget();
        }
        
        private async UniTask ActionAsync()
        {
            foreach (var enemy in Owner.MapManager.EnemyManager.AttackEnemies)
            {
                enemy.SetTarget(Owner.MapManager.PlayerController);
                await enemy.AttackAsync();
            }
            
            // 攻撃リストをクリア
            Owner.MapManager.EnemyManager.ClearAttackEnemy();
            // 攻撃終了後、ターン終了Stateに遷移
            Owner.ChangeState(Owner.TurnEndState);
        }
    }
}