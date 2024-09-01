using System.Threading;
using UniRx;
using UnityEngine;
using Utils;
using Utils.Interface;

namespace Controller.Logic.State
{
    /// <summary>
    /// プレイヤーの入力State
    /// </summary>
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
            
            // Playerの現在位置を取得
            var transform = Owner.MapManager.PlayerController.transform;
            
            // キー入力を取得
            // 入力を監視
            _inputManager
                .InputObservable
                .Where(_ => Owner.CurrentState == this && transform != null)
                .Subscribe(input =>
                {
                    // トークンをキャンセル
                    _tokenSource?.Cancel();
                    // トークンを再生成
                    _tokenSource = new CancellationTokenSource();
                    
                    // 移動先の座標を計算
                    Vector3 targetPosition = transform.position + new Vector3(input.x, input.y, 0);
                    
                    // 通行可能でなければ移動しない
                    if(!Owner.MapManager.CanThrough((int)targetPosition.x, (int)targetPosition.y))
                    {
                        // ターンは消費しない
                        return;
                    }

                    // 入力をプレイヤーに渡す
                    Owner.MapManager.PlayerController.SetNextPosition(input);
                    
                    // Animator更新
                    Owner.MapManager.PlayerController.SetAnimator(input.normalized);

                    // 敵に攻撃可能かどうか判定
                    if (Owner.MapManager.IsExistEnemy(Owner.MapManager.PlayerController.NextPosition, out var enemy))
                    {
                        // ターゲットをセット
                        Owner.MapManager.PlayerController.SetTarget(enemy);
                        
                        // 配置移動は行わない
                        Owner.MapManager.PlayerController.SetNextPosCurrent();
                        // 敵に攻撃するstateに遷移
                        Owner.ChangeState(Owner.PlayerAttackState);
                    }
                    else
                    {
                        // 移動Stateに遷移
                        Owner.ChangeState(Owner.PlayerMoveState);
                    }
                })
                .AddTo(_disposable);
            
            // インベントリを開く
            _inputManager
                .OnSwitchInventoryOpenRx
                .Where(_ => Owner.CurrentState == this)
                .Subscribe(_ =>
                {
                    Debug.Log("インベントリを開く");
                    
                    // インベントリを開く
                    // Owner.UiView.SwitchActiveInventoryUi();

                    // 開いている間は動かない
                    // もう一度押したら閉じる
                })
                .AddTo(_disposable);
            
            // アイテムを使用
            _inputManager
                .OnUseItemRx
                .Where(_ => Owner.CurrentState == this)
                .Subscribe(_ =>
                {
                    Debug.Log("アイテムを使用");
                    
                    // アイテムを使用
                    // Owner.MapManager.PlayerController.UseItem();
                })
                .AddTo(_disposable);
        }
        
        public override void OnUpdate()
        {
            // 入力が取れなければ終了
            if (_inputManager == null)
            {
                Debug.LogError("InputManager is null.");
                
                OnExit();
            }
        }
        
        public override void OnExit()
        {
            _disposable?.Dispose();
        }
    }
}