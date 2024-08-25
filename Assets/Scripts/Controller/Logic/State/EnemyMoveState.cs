using Cysharp.Threading.Tasks;
using UniRx;

namespace Controller.Logic.State
{
    public class EnemyMoveState
        : StateBase
    {
        public EnemyMoveState(in StateManager owner) : base(in owner)
        {
        }
        
        public override void OnEnter()
        {
            ExecuteActions();
        }

        private void ExecuteActions()
        {
            // 敵の移動処理
            foreach (var enemy in Owner.MapManager.EnemyManager.Enemies)
            {
                // Playerが攻撃範囲にいる場合、攻撃
                if (enemy.CanAttackPlayer(Owner.MapManager.PlayerController))
                {
                    // 攻撃実行するリストに追加
                    Owner.MapManager.EnemyManager.AddAttackEnemy(enemy);
                    
                    // 移動しないので次の敵へ
                    continue;
                }
                
                // Playerを追いかける
                // A*アルゴリズムで移動
                enemy.FindPath(Owner.MapManager);
                
                var token = new CancellationDisposable().Token;
                
                // 移動処理
                enemy.MoveAsync(token).Forget();
            }
            
            // 移動終了後、敵攻撃Stateに遷移
            Owner.ChangeState(Owner.EnemyAttackState);
        }
    }
}