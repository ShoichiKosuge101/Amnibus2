using Constants;
using Dungeons;
using UniRx;
using UnityEngine;

namespace Manager.Interface
{
    public interface IMapManager
    {
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mapDisplay"></param>
        void Initialize(Layer2D map, MapDisplay mapDisplay);
        
        Layer2D　CurrentMap { get; set; }
        Subject<Unit> OnGoalReachedRx { get; }
        Subject<Vector2Int> OnItemPickedUpRx { get; }
        
        void SetMap(Layer2D map);
        Layer2D GetMap();
        
        void UpdateMap(int x, int y, MapTile tile);
        void UpdatePlayerPosition(Vector2Int beforePosition, Vector2Int afterPosition);
        /// <summary>
        /// プレイヤー生成位置の取得
        /// </summary>
        Vector2 GetSpawnPlayerPosition();
        
        Vector2Int GetRandomFloor();

        bool CanThrough(int x, int y);

        void DiscardObj(Vector2Int position);
    }
}