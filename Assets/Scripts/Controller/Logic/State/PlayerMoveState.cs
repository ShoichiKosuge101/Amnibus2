using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Controller.Logic.State
{
    public class PlayerMoveState
        : StateBase
    {
        public PlayerMoveState(in StateManager owner) : base(in owner)
        {
        }
        
        public override void OnEnter()
        {
            // トークン生成
            var cancellationToken = new CancellationDisposable().Token;
            
            // 移動処理
            MoveAsync(cancellationToken).Forget();
        }

        private async UniTask MoveAsync(CancellationToken token)
        {
            // 入力方向に移動
            await Owner.MapManager.PlayerController.MoveAsync(token);
            
            // 移動終了後、敵の移動Stateに遷移
            Owner.ChangeState(Owner.EnemyMoveState);
        }
    }
}