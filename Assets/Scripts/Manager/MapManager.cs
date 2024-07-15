﻿using Constants;
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

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="map"></param>
        public void Initialize(Layer2D map)
        {
            CurrentMap = map;
            
            // ゴールを配置
            PlaceGoal();
            
            // エネミーを配置
            PlaceEnemies(5);
            
            // アイテムを配置
            PlaceItems(3);
            
            // プレイヤーの生成位置を取得
            var playerPosition = GetSpawnPlayerPosition();
            // プレイヤーの座標を登録
            UpdateMap((int)playerPosition.x, (int)playerPosition.y, MapTile.Player);
        }

        public void SetMap(Layer2D map)
        {
            CurrentMap = map;
        }
        public Layer2D GetMap()
        {
            return CurrentMap;
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
            UpdateMap(beforePosition.x, beforePosition.y, MapTile.Floor);
            
            // ゴールに到達したかどうか
            if (CurrentMap.Get(afterPosition.x, afterPosition.y).IsGoal)
            {
                OnGoalReachedRx.OnNext(Unit.Default);
            }
            
            // TODO: プレイヤーフラグを渡すが正しい
            // 移動後の座標をプレイヤーにする
            UpdateMap(afterPosition.x, afterPosition.y, MapTile.Player);
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

        /// <summary>
        /// プレイヤーの生成位置を返す
        /// </summary>
        /// <returns></returns>
        public Vector2 GetSpawnPlayerPosition()
        {
            return GetRandomFloor();
        }
        
        
        /// <summary>
        /// ゴールを配置
        /// </summary>
        private void PlaceGoal()
        {
            // 床の位置を全部集める
            var floorPositions = CurrentMap.GetFloors();
            
            // ランダムにゴールの位置を決める
            Vector2Int _goalPosition = floorPositions[Random.Range(0, floorPositions.Count)];
            
            UpdateMap(_goalPosition.x, _goalPosition.y, MapTile.Goal);
        }
        
        /// <summary>
        /// アイテムの配置を決める
        /// </summary>
        private void PlaceItems(int itemCount)
        {
            // 要求数分アイテムを配置
            for (int i = 0; i < itemCount; ++i)
            {
                PlaceItem();
            }
        }
        
        /// <summary>
        /// アイテムを配置
        /// </summary>
        private void PlaceItem()
        {
            // 床の位置を全部集める
            var floorPositions = CurrentMap.GetFloors();
            
            // ランダムにアイテムの位置を決める
            Vector2Int _itemPosition = floorPositions[Random.Range(0, floorPositions.Count)];
            
            UpdateMap(_itemPosition.x, _itemPosition.y, MapTile.Treasure);
        }
        
        /// <summary>
        /// 敵の配置を決める
        /// </summary>
        /// <param name="enemyCount"></param>
        private void PlaceEnemies(int enemyCount)
        {
            // 要求数分敵を配置
            for (int i = 0; i < enemyCount; ++i)
            {
                PlaceEnemy();
            }
        }

        private void PlaceEnemy()
        {
            // 床の位置を全部集める
            var floorPositions = CurrentMap.GetFloors();
            
            // ランダムに敵の位置を決める
            Vector2Int _enemyPosition = floorPositions[Random.Range(0, floorPositions.Count)];
            
            UpdateMap(_enemyPosition.x, _enemyPosition.y, MapTile.Enemy);
        }

    }
}