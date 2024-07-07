using UniRx;
using UnityEngine;

namespace MapObject
{
    /// <summary>
    /// ゴールオブジェクト
    /// </summary>
    public class Goal
        : MonoBehaviour
    {
        public static Subject<Unit> OnGoalReachedRx { get; } = new Subject<Unit>();
        
        /// <summary>
        /// プレイヤーがゴールに到達した際の処理
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Player"))
            {
                // ゴール到達通知
                OnGoalReachedRx.OnNext(Unit.Default);
            }
        }
    }
}