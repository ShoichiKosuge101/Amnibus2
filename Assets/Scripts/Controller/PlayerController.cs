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
        
        private readonly Subject<(Vector2Int, Vector2Int)> _onPositionChanged = new Subject<(Vector2Int, Vector2Int)>();
        public IObservable<(Vector2Int, Vector2Int)> OnPositionChanged => _onPositionChanged;

        private CancellationTokenSource _tokenSource;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
　           // インプットマネージャーを取得
            var inputManager = ServiceLocator.Instance.Resolve<IInputManager>();
            // マップ管理クラスを取得
            var mapManager = ServiceLocator.Instance.Resolve<IMapManager>();
            
            // 入力を監視
            inputManager
                .InputObservable
                .Where(_ => !_isMoving)
                .TakeUntilDestroy(this)
                .Subscribe(input =>
                {
                    // トークンをキャンセル
                    _tokenSource?.Cancel();
                    // トークンを再生成
                    _tokenSource = new CancellationTokenSource();
                    
                    // 移動先の座標を計算
                    Vector3 targetPosition = transform.position + new Vector3(input.x, input.y, 0);

                    // 通行可能でなければ移動しない
                    if(!mapManager.CanThrough((int)targetPosition.x, (int)targetPosition.y))
                    {
                        return;
                    }
                    
                    // 移動処理
                    MoveAsync(new Vector3(input.x, input.y, 0), _tokenSource.Token).Forget();
                });
        }
        
        /// <summary>
        /// 移動処理
        /// </summary>
        private async UniTask MoveAsync(Vector3 direction, CancellationToken token)
        {
            // 移動中フラグを立てる
            _isMoving = true;
            
            // 今の座標を保持
            Vector2Int beforePosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            
            // 移動先の座標を計算
            Vector3 targetPosition = transform.position + direction;

            try
            {
                // 移動先に到達するまでループ
                while (
                    transform != null 
                    && transform.position != targetPosition)
                {
                    // キャンセルされたら処理を終了
                    token.ThrowIfCancellationRequested();
                
                    transform.position = Vector3
                        .MoveTowards(
                            transform.position, 
                            targetPosition, 
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
