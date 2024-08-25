using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

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
            InitializeAsync().Forget();
        }

        private async UniTask InitializeAsync()
        {
            // TODO: 敵の移動処理
            foreach (var enemy in Owner.MapManager.EnemyManager.Enemies)
            {
                // Playerを追いかける
                // A*アルゴリズムで移動
                enemy.FindPath(Owner.MapManager);
                // Vector2 direction = Owner.MapManager.PlayerController.transform.position - enemy.transform.position;
                // // ただし、斜め移動はしない
                // if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                // {
                //     direction.y = 0;
                // }
                // else
                // {
                //     direction.x = 0;
                // }
                // // １マス移動する
                // direction = direction.normalized;
                //
                // // 移動先を設定
                // enemy.SetNextPosition(direction);
                
                var token = new CancellationDisposable().Token;
                
                // 移動処理
                enemy.MoveAsync(token).Forget();
            }
            
            // 移動終了後、ターン終了Stateに遷移
            Owner.ChangeState(Owner.TurnEndState);
        }
    }
}