using System.Collections.Generic;
using System.Linq;
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
        public Transform parent;
        
        // Prefab
        public PlayerController playerPrefab;
        public GameObject goalPrefab;

        // ObjectPool
        public ObjectPool floorPool;
        public ObjectPool wallPool;
        public ObjectPool enemyPool;
        public ObjectPool item01Pool;
        
        private PlayerController _player;
        
        private Dictionary<Vector2, List<GameObject>> objectsAtPositions = new ();

        private IMapManager _mapManager;
        
        /// <summary>
        /// プレイヤーの生成
        /// </summary>
        /// <param name="position"></param>
        private void SpawnPlayer(in Vector2 position)
        {
            _player = Instantiate(playerPrefab, position, Quaternion.identity, parent);
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
        /// 敵を取得
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EnemyController> GetEnemies()
        {
            return objectsAtPositions.Values
                .SelectMany(objs => 
                    objs.Where(obj => obj.GetComponent<EnemyController>() != null)
                    .Select(obj => obj.GetComponent<EnemyController>())
                    );
        }
        
        /// <summary>
        /// マップを表示
        /// </summary>
        public void DisplayMap(IMapManager mapManager)
        {
            // TODO: 一旦ここで変数に格納
            _mapManager = mapManager;
            
            var layer2D = mapManager.GetMap();

            for (int y = 0; y < layer2D.Height; y++)
            {
                for (int x = 0; x < layer2D.Width; x++)
                {
                    Vector2 position = new Vector2(x, y);
                    var objectsAtPosition = new List<GameObject>();
                    
                    var mapTile = GetMapTile(layer2D, x, y);
                    if(mapTile == MapTile.Wall)
                    {
                        GameObject wallPrefab = wallPool.GetObject(parent);
                        wallPrefab.transform.position = position;
                        // 登録
                        objectsAtPosition.Add(wallPrefab);
                        
                        continue;
                    }
                    if(mapTile == MapTile.Floor)
                    {
                        GameObject floorPrefab = floorPool.GetObject(parent);
                        floorPrefab.transform.position = position;
                        // 登録
                        objectsAtPosition.Add(floorPrefab);
                        
                        continue;
                    }
                    if (mapTile == MapTile.Goal)
                    {
                        Instantiate(goalPrefab, position, Quaternion.identity, parent);
                        // 登録
                        objectsAtPosition.Add(goalPrefab);
                        
                        continue;
                    }
                    if(mapTile == MapTile.Player)
                    {
                        // まず床を描画
                        GameObject floorPrefab = floorPool.GetObject(parent);
                        floorPrefab.transform.position = position;
                        // 登録
                        objectsAtPosition.Add(floorPrefab);
                        
                        // その上にプレイヤーを描画
                        SpawnPlayer(position);
                    }
                    if (mapTile == MapTile.Enemy)
                    {
                        // まず床を描画
                        GameObject floorPrefab = floorPool.GetObject(parent);
                        floorPrefab.transform.position = position;
                        // 登録
                        objectsAtPosition.Add(floorPrefab);
                        
                        // その上に敵を描画
                        GameObject enemyPrefab = enemyPool.GetObject(parent);
                        enemyPrefab.transform.position = position;
                        // 登録
                        objectsAtPosition.Add(enemyPrefab);
                    }
                    if (mapTile == MapTile.Treasure)
                    {
                        // まず床を描画
                        GameObject floorPrefab = floorPool.GetObject(parent);
                        floorPrefab.transform.position = position;
                        // 登録
                        objectsAtPosition.Add(floorPrefab);
                        
                        // その上にアイテムを描画
                        GameObject item01Prefab = item01Pool.GetObject(parent);
                        item01Prefab.transform.position = position;
                        // 登録
                        objectsAtPosition.Add(item01Prefab);
                    }
                    
                    // 登録
                    objectsAtPositions[position] = objectsAtPosition;
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

        /// <summary>
        /// 指定のオブジェクトを返却
        /// </summary>
        /// <param name="position"></param>
        /// <param name="mapTile"></param>
        public void ReleaseObject(Vector2 position, MapTile mapTile)
        {
            var currentTile = GetMapTile(_mapManager.CurrentMap, (int)position.x, (int)position.y);
            Debug.Log($"ReleaseObject: {currentTile}");
            
            // 登録されているオブジェクトを削除
            if (objectsAtPositions.TryGetValue(position, out var objAtPosition))
            {
                // 指定のタグを持つオブジェクトを検索
                var obj = objAtPosition.FirstOrDefault(obj => obj.CompareTag(mapTile.GetTag()));
                if (obj != null)
                {
                    // プールに返却
                    ReturnObjectToPool(obj);
                    
                    // 登録から削除
                    objAtPosition.Remove(obj);
                }
                
                // 登録を更新
                objectsAtPositions[position] = objAtPosition;
            }
        }

        /// <summary>
        /// オブジェクトをプールに返却
        /// </summary>
        /// <param name="obj"></param>
        private void ReturnObjectToPool(GameObject obj)
        {
            // タグによってプールを切り替え
            if (obj.CompareTag(MapTile.Wall.GetTag()))
            {
                wallPool.ReleaseObject(obj);
            }
            if (obj.CompareTag(MapTile.Floor.GetTag()))
            {
                floorPool.ReleaseObject(obj);
            }
            if (obj.CompareTag(MapTile.Enemy.GetTag()))
            {
                enemyPool.ReleaseObject(obj);
            }
            if (obj.CompareTag(MapTile.Treasure.GetTag()))
            {
                item01Pool.ReleaseObject(obj);
            }
        }
    }
}