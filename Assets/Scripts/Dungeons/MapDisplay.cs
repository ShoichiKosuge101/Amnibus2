using Constants;
using Controller;
using Manager.Interface;
using UnityEngine;
using Utils;

namespace Dungeons
{
    /// <summary>
    /// マップ描画
    /// </summary>
    public class MapDisplay
        : MonoBehaviour
    {
        // Prefab
        public PlayerController playerPrefab;
        public GameObject goalPrefab;

        // ObjectPool
        public ObjectPool floorPool;
        public ObjectPool wallPool;
        public ObjectPool item01Pool;
        
        private PlayerController _player;
        
        /// <summary>
        /// プレイヤーの生成
        /// </summary>
        /// <param name="position"></param>
        private void SpawnPlayer(in Vector2 position)
        {
            _player = Instantiate(playerPrefab, position, Quaternion.identity);
        }
        
        /// <summary>
        /// プレイヤーを取得
        /// </summary>
        /// <returns></returns>
        public PlayerController GetPlayer()
        {
            return _player;
        }
        
        /// <summary>
        /// マップを表示
        /// </summary>
        public void DisplayMap(IMapManager mapManager)
        {
            var layer2D = mapManager.GetMap();

            for (int y = 0; y < layer2D.Height; y++)
            {
                for (int x = 0; x < layer2D.Width; x++)
                {
                    Vector2 position = new Vector2(x, y);
                    
                    var mapTile = GetMapTile(layer2D, x, y);
                    if(mapTile == MapTile.Wall)
                    {
                        GameObject wallPrefab = wallPool.GetObject();
                        wallPrefab.transform.position = position;
                        continue;
                    }
                    if(mapTile == MapTile.Floor)
                    {
                        GameObject floorPrefab = floorPool.GetObject();
                        floorPrefab.transform.position = position;
                        continue;
                    }
                    if (mapTile == MapTile.Goal)
                    {
                        Instantiate(goalPrefab, position, Quaternion.identity);
                    }
                    if(mapTile == MapTile.Player)
                    {
                        // まず床を描画
                        GameObject floorPrefab = floorPool.GetObject();
                        floorPrefab.transform.position = position;
                        
                        // その上にプレイヤーを描画
                        SpawnPlayer(position);
                    }
                    if (mapTile == MapTile.Treasure)
                    {
                        // まず床を描画
                        GameObject floorPrefab = floorPool.GetObject();
                        floorPrefab.transform.position = position;
                        
                        // その上にアイテムを描画
                        GameObject item01Prefab = item01Pool.GetObject();
                        item01Prefab.transform.position = position;
                    }
                }
            }
        }

        /// <summary>
        /// マップ情報を取得
        /// </summary>
        /// <param name="layer2D"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static MapTile GetMapTile(Layer2D layer2D, int x, int y)
        {
            return layer2D.Get(x, y);
        }
    }
}