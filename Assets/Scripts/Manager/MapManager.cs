using Constants;
using Dungeons;
using Manager.Interface;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager
{
    /// <summary>
    /// マップ管理
    /// </summary>
    public class MapManager
        : IMapManager
    {
        public Layer2D CurrentMap { get; set; } = new(0, 0);
        
        public Subject<Unit> OnGoalReachedRx { get; } = new();

        public void SetMap(Layer2D map)
        {
            CurrentMap = map;
        }

        public void UpdateMap(int x, int y, MapTile tile)
        {
            CurrentMap.Set(x, y, tile);
        }

        /// <summary>
        /// プレイヤーの座標を更新
        /// </summary>
        /// <param name="beforePosition"></param>
        /// <param name="afterPosition"></param>
        public void UpdatePlayerPosition(Vector2Int beforePosition, Vector2Int afterPosition)
        {
            // 移動前の座標を床にする
            CurrentMap.Set(beforePosition.x, beforePosition.y, MapTile.Floor);
            
            // ゴールに到達したかどうか
            if (CurrentMap.Get(afterPosition.x, afterPosition.y).IsGoal)
            {
                OnGoalReachedRx.OnNext(Unit.Default);
            }
            
            // TODO: プレイヤーフラグを渡すが正しい
            // 移動後の座標をプレイヤーにする
            CurrentMap.Set(afterPosition.x, afterPosition.y, MapTile.Player);
        }

        public Layer2D GetMap()
        {
            return CurrentMap;
        }

        /// <summary>
        /// ランダムな床の座標を取得
        /// </summary>
        /// <returns></returns>
        public Vector2Int GetRandomFloor()
        {
            // ランダムな床の座標を取得
            while (true)
            {
                var x = Random.Range(0, CurrentMap.Width);
                var y = Random.Range(0, CurrentMap.Height);
                if (CurrentMap.Get(x, y).IsFloor)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        /// <summary>
        /// 通行可能かどうか
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool CanThrough(int x, int y)
        {
            return !CurrentMap.Get(x, y).IsWall;
        }
    }
}