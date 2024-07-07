using Constants;
using Dungeons;
using UniRx;
using UnityEngine;

namespace Manager.Interface
{
    public interface IMapManager
    {
        Layer2D　CurrentMap { get; set; }
        Subject<Unit> OnGoalReachedRx { get; }
        
        void SetMap(Layer2D map);
        
        void UpdateMap(int x, int y, MapTile tile);
        void UpdatePlayerPosition(Vector2Int beforePosition, Vector2Int afterPosition);
        
        Layer2D GetMap();
        
        Vector2Int GetRandomFloor();

        bool CanThrough(int x, int y);
    }
}