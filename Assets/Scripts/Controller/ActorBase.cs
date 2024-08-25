using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class ActorBase
        : MonoBehaviour
    {
        [SerializeField] 
        private float speed = 5.0f;
        
        // 移動中かどうか
        public bool IsMoving { get; private set; }
        
        // 次の座標
        public Vector2 NextPosition { get; private set; }
        
        /// <summary>
        /// 攻撃対象
        /// </summary>
        private ActorBase _target;
        
        private readonly Subject<(Vector2Int, Vector2Int)> _onPositionChanged = new Subject<(Vector2Int, Vector2Int)>();
        public IObservable<(Vector2Int, Vector2Int)> OnPositionChanged => _onPositionChanged;

        private CancellationTokenSource _tokenSource;
        
        public void SetNextPosition(Vector2 nextPosition)
        {
            NextPosition = transform.position + (Vector3)nextPosition;
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
                    && transform.position != (Vector3)NextPosition)
                {
                    // キャンセルされたら処理を終了
                    token.ThrowIfCancellationRequested();
                
                    transform.position = Vector3
                        .MoveTowards(
                            transform.position, 
                            NextPosition, 
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
        
        public void SetNextPosCurrent()
        {
            NextPosition = transform.position;
        }
        
        public async UniTask AttackAsync()
        {
            NextPosition = transform.position;
            
            Debug.Log($"<color=yellow>{name} は {_target.name}　を攻撃</color>");
            
            // 攻撃アクション
            await ActionAttackAsync();
            
            // 実行したら対象をクリア
            _target = null;
        }

        /// <summary>
        /// 攻撃アクション
        /// </summary>
        private async UniTask ActionAttackAsync()
        {
            // targetの方向にDOTweenを使って攻撃アニメーションを再生
            // 一瞬targetのほうに移動して、元に戻るアニメーション
            await transform.DOPunchPosition(
                    (_target.transform.position - transform.position).normalized, 
                    0.5f, 
                    10, 
                    0.5f
                )
                .SetLink(gameObject)
                .AsyncWaitForCompletion();
        }
        
        /// <summary>
        /// 攻撃対象をセット
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(in ActorBase target)
        {
            _target = target;
        }
    }
}