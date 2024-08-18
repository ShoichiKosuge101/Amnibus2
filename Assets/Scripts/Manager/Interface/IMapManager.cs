using Constants;
using Controller;
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
        PlayerController PlayerController { get; }
        EnemyManager EnemyManager { get; }
        
        void SetMap(Layer2D map);
        Layer2D GetMap();
        
        void UpdateMap(int x, int y, MapTile tile);
        void UpdatePlayerPosition(Vector2Int beforePosition, Vector2Int afterPosition);
        /// <summary>
        /// プレイヤー生成位置の取得
        /// </summary>
        Vector2 GetSpawnPlayerPosition();

        /// <summary>
        /// プレイヤー情報の登録
        /// </summary>
        /// <param name="playerController"></param>
        void SetPlayer(PlayerController playerController);
        
        Vector2Int GetRandomFloor();

        bool CanThrough(int x, int y);

        void DiscardObj(Vector2Int position, MapTile mapTile);
        
        void RideOnObj(Vector2Int position, MapTile mapTile);
        
        void RemoveObj(Vector2Int position, MapTile mapTile);
    }
}