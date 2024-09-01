using System;
using System.Threading;
using Controller.Animator;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Manager.Service;
using UniRx;
using UnityEngine;
using Utils;

namespace Controller
{
    public class ActorBase
        : MonoBehaviour
    {
        [SerializeField] 
        private float speed = 5.0f;
        
        [SerializeField]
        private ActorData _actorData;
        public int MaxHp => _actorData.Hp;
        
        [SerializeField]
        protected PlayerAnimatorController _animatorController;
        
        /// <summary>
        /// 現在のHP
        /// </summary>
        protected readonly ReactiveProperty<int> _currentHpRx = new ReactiveProperty<int>(0);
        
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

        /// <summary>
        /// 倒された時のイベント
        /// </summary>
        private readonly Subject<Unit> _onDefeatRx = new Subject<Unit>();
        public IObservable<Unit> OnDefeatRx => _onDefeatRx;

        private CancellationTokenSource _tokenSource;

        private void Start()
        {
            // プレイヤーだった時はHpServiceから取り出す
            if (this is PlayerController)
            {
                var playerHpService = ServiceLocator.Instance.Resolve<PlayerHpService>();
                // 初期化(一度だけ実行される)
                playerHpService.Initialize(_actorData.Hp);
                
                _currentHpRx.Value = playerHpService.CurrentHp;
            }
            else
            {
                // HPを格納
                _currentHpRx.Value = _actorData.Hp;
            }
        }

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
            // 対象がいない場合は処理を終了
            if (_target == null)
            {
                return;
            }
            
            NextPosition = transform.position;
            
            Debug.Log($"<color=yellow>{name} は {_target.name}　を攻撃</color>");
            
            // 攻撃アクション
            await ActionAttackAsync();

            // ダメージ処理
            _target.Damage(_actorData.Attack);

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
            // DOPunchPosition(攻撃方向, 振動時間, 振動回数, 振動の強さ)
            await transform.DOPunchPosition(
                    (_target.transform.position - transform.position).normalized, 
                    0.3f, 
                    1, 
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
        
        /// <summary>
        /// ダメージ処理
        /// </summary>
        /// <param name="damage"></param>
        public void Damage(int damage)
        {
            // ダメージ処理
            // 0未満にならないようにする
            _currentHpRx.Value = Math.Max(0, _currentHpRx.Value - damage);
            
            if (_currentHpRx.Value <= 0)
            {
                Debug.Log($"<color=red>{name} は倒れた</color>");
                
                _onDefeatRx.OnNext(Unit.Default);
                    
                // 敵なら直ぐに削除
                if (this is EnemyController)
                {
                    Delete();
                }
            }
        }
        
        public void Delete()
        {
            Destroy(gameObject);
        }
    }
}