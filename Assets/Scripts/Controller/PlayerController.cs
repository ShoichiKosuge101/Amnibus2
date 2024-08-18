using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Manager.Interface;
using UnityEngine;
using UniRx;
using Utils;
using Utils.Interface;

namespace Controller
{
    /// <summary>
    /// プレイヤーコントローラー
    /// </summary>
    public class PlayerController 
        : MonoBehaviour
    {
        [SerializeField] 
        private float speed = 5.0f;
        
        // 移動中かどうか
        private bool _isMoving;
        
        // 次の座標
        private Vector2 _nextPosition;
        
        private readonly Subject<(Vector2Int, Vector2Int)> _onPositionChanged = new Subject<(Vector2Int, Vector2Int)>();
        public IObservable<(Vector2Int, Vector2Int)> OnPositionChanged => _onPositionChanged;

        private CancellationTokenSource _tokenSource;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
　           // // インプットマネージャーを取得
            // var inputManager = ServiceLocator.Instance.Resolve<IInputManager>();
            // // マップ管理クラスを取得
            // var mapManager = ServiceLocator.Instance.Resolve<IMapManager>();
            //
            // // 入力を監視
            // inputManager
            //     .InputObservable
            //     .Where(_ => !_isMoving)
            //     .TakeUntilDestroy(this)
            //     .Subscribe(input =>
            //     {
            //         // トークンをキャンセル
            //         _tokenSource?.Cancel();
            //         // トークンを再生成
            //         _tokenSource = new CancellationTokenSource();
            //         
            //         // 移動先の座標を計算
            //         Vector3 targetPosition = transform.position + new Vector3(input.x, input.y, 0);
            //
            //         // 通行可能でなければ移動しない
            //         if(!mapManager.CanThrough((int)targetPosition.x, (int)targetPosition.y))
            //         {
            //             return;
            //         }
            //         
            //         // 移動処理
            //         MoveAsync(new Vector3(input.x, input.y, 0), _tokenSource.Token).Forget();
            //     });
        }
        
        public void SetNextPosition(Vector2 nextPosition)
        {
            _nextPosition = transform.position + (Vector3)nextPosition;
        }
        
        /// <summary>
        /// 移動処理
        /// </summary>
        public async UniTask MoveAsync(CancellationToken token)
        {
            // 移動中フラグを立てる
            _isMoving = true;
            
            // 今の座標を保持
            Vector2Int beforePosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            
            try
            {
                // 移動先に到達するまでループ
                while (
                    transform != null 
                    && transform.position != (Vector3)_nextPosition)
                {
                    // キャンセルされたら処理を終了
                    token.ThrowIfCancellationRequested();
                
                    transform.position = Vector3
                        .MoveTowards(
                            transform.position, 
                            _nextPosition, 
                            Time.deltaTime * speed
                        );
                
                    // 1フレーム待機
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }                
            }
            catch (OperationCanceledException)
            {
                // キャンセルされたら処理を終了
                return;
            }
            
            // 移動中フラグを下げる
            _isMoving = false;
            
            // 移動後の座標を通知
            _onPositionChanged.OnNext(
                (
                    beforePosition, 
                    new Vector2Int((int)transform.position.x, (int)transform.position.y))
                );
        }
    }
}
