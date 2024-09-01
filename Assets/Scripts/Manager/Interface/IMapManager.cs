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
        
        // 位置情報とタイル種別からオブジェクト名を取得
        string GetObjTag(int x, int y, MapTile mapTile);
        
        Vector2Int GetRandomFloor();

        bool CanThrough(int x, int y);
        
        bool IsExistEnemy(Vector2 position, out EnemyController enemy);
        bool IsExistEnemyInNextPosition(Vector2 nextPosition);
        bool IsExistPlayer(int x, int y);

        void DiscardObj(Vector2Int position, MapTile mapTile);
        
        void RideOnObj(Vector2Int position, MapTile mapTile);
        
        void RemoveObj(Vector2Int position, MapTile mapTile);
    }
}