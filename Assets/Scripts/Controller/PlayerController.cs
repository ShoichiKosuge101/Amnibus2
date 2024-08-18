using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

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
        public bool IsMoving { get; private set; }
        
        // 次の座標
        private Vector2 _nextPosition;
        
        private readonly Subject<(Vector2Int, Vector2Int)> _onPositionChanged = new Subject<(Vector2Int, Vector2Int)>();
        public IObservable<(Vector2Int, Vector2Int)> OnPositionChanged => _onPositionChanged;

        private CancellationTokenSource _tokenSource;
        
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
            IsMoving = true;
            
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
            IsMoving = false;
            
            // 移動後の座標を通知
            _onPositionChanged.OnNext(
                (
                    beforePosition, 
                    new Vector2Int((int)transform.position.x, (int)transform.position.y))
                );
        }
    }
}
