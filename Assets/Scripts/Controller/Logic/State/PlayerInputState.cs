using System.Threading;
using UniRx;
using Utils;
using Utils.Interface;

namespace Controller.Logic.State
{
    public class PlayerInputState
        : StateBase
    {
        private IInputManager _inputManager;
        private CompositeDisposable _disposable;
        
        private CancellationTokenSource _tokenSource;
        
        public PlayerInputState(in StateManager owner) : base(in owner)
        {
        }

        public override void OnEnter()
        {
            // 購読の再初期化
            _disposable = new CompositeDisposable();
            
            // インプットを取得する
            _inputManager = ServiceLocator.Instance.Resolve<IInputManager>();
            
            // トークンを生成
            _tokenSource = new CancellationTokenSource();
            
            // キー入力を取得
            // 入力を監視
            _inputManager
                .InputObservable
                .Subscribe(input =>
                {
                    // トークンをキャンセル
                    _tokenSource?.Cancel();
                    // トークンを再生成
                    _tokenSource = new CancellationTokenSource();
                    
                    // 入力をプレイヤーに渡す
                    Owner.MapManager.PlayerController.SetNextPosition(input);
                    
                    // 移動Stateに遷移
                    Owner.ChangeState(Owner.PlayerMoveState);
                    
                    // // 移動先の座標を計算
                    // Vector3 targetPosition = transform.position + new Vector3(input.x, input.y, 0);
                    //
                    // // 通行可能でなければ移動しない
                    // if(!mapManager.CanThrough((int)targetPosition.x, (int)targetPosition.y))
                    // {
                    //     return;
                    // }
                    //
                    // // 移動処理
                    // MoveAsync(new Vector3(input.x, input.y, 0), _tokenSource.Token).Forget();
                })
                .AddTo(_disposable);
        }
        
        public override void OnUpdate()
        {
            // 入力購読
            if (_inputManager == null)
            {
                OnExit();
                return;
            }
        }
        
        public override void OnExit()
        {
            _disposable?.Dispose();
        }
    }
}