using Dungeons;
using Manager.Interface;
using UnityEngine;

namespace Controller
{
    public class EnemyController
        : ActorBase
    {
        // 敵の攻撃
        // 攻撃範囲はプレイヤーの周囲1マス
        private readonly Vector2Int[] _attackRange = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        
        /// <summary>
        /// Playerが攻撃範囲にいるかどうか
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool CanAttackPlayer(PlayerController player)
        {
            if (player == null)
            {
                return false;
            }
            
            foreach (var range in _attackRange)
            {
                Vector2Int currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
                if (player.NextPosition == currentPosition + range)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 移動経路を見つけて移動する
        /// </summary>
        /// <param name="mapManager"></param>
        public void FindPath(IMapManager mapManager)
        {
            var aStarPath = new AStarPath();
            var nextPosition = aStarPath.AStarSearchPathFinding(
                mapManager, 
                Vector2Int.FloorToInt(transform.position), 
                Vector2Int.FloorToInt(mapManager.PlayerController.NextPosition));
            // 次の移動先に変換する為、現在の場所との差分を取得
            var diff = (nextPosition - (Vector2)transform.position).normalized;
            
            // 重なり問題の対処
            // 移動先にすでに敵がいる場合は移動しない
            if (mapManager.IsExistEnemyInNextPosition(nextPosition))
            {
                // 今の場所に居続ける
                return;
            }
            
            // 移動先を設定
            SetNextPosition(diff);
            
            // 向きの設定
            SetDirection(diff);
        }
        
        public void SetDirection(Vector2 direction)
        {
            _animatorController.SetKeyDirection(direction);
        }
    }
}