using Dungeons;
using Manager.Interface;
using UnityEngine;

namespace Controller
{
    public class EnemyController
        : ActorBase
    {
        public void FindPath(IMapManager mapManager)
        {
            var aStarPath = new AStarPath();
            var lastPos = aStarPath.AStarSearchPathFinding(
                mapManager, 
                Vector2Int.FloorToInt(transform.position), 
                Vector2Int.FloorToInt(mapManager.PlayerController.NextPosition));
            // 次の移動先に変換する為、現在の場所との差分を取得
            var diff = lastPos - (Vector2)transform.position;
            // 移動先を設定
            SetNextPosition(diff);
        }
    }
}