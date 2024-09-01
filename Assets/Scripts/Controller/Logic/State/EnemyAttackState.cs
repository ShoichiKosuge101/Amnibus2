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
            // プレイヤーが移動中の場合、待機
            await UniTask.WaitWhile(() => Owner.MapManager.PlayerController.IsMoving);
            
            foreach (var enemy in Owner.MapManager.EnemyManager.AttackEnemies)
            {
                enemy.SetTarget(Owner.MapManager.PlayerController);
                
                var targetDirection = (Owner.MapManager.PlayerController.transform.position - enemy.transform.position).normalized;
                
                // 向きの設定
                enemy.SetDirection(targetDirection);

                // 攻撃
                await enemy.AttackAsync();
            }
            
            // 攻撃リストをクリア
            Owner.MapManager.EnemyManager.ClearAttackEnemy();
            // 攻撃終了後、ターン終了Stateに遷移
            Owner.ChangeState(Owner.TurnEndState);
        }
    }
}