using System.Threading;
using Cysharp.Threading.Tasks;
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

        private CancellationTokenSource _tokenSource;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
           // インプットマネージャーを取得
            var inputManager = ServiceLocator.Instance.Resolve<IInputManager>();
            
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
            
            // 移動先の座標を計算
            Vector3 targetPosition = transform.position + direction;

            // 移動先に到達するまでループ
            while (transform.position != targetPosition)
            {
                transform.position = Vector3
                    .MoveTowards(
                        transform.position, 
                        targetPosition, 
                        Time.deltaTime * speed
                    );
                
                // 1フレーム待機
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
            
            // 移動中フラグを下げる
            _isMoving = false;
        }
    }
}
