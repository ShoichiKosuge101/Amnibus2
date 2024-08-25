using System;
using System.Collections.Generic;
using System.Linq;
using Constants;
using Manager.Interface;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeons
{
    /// <summary>
    /// マス情報
    /// </summary>
    public class CellInfo
    {
        // マスの座標
        public Vector2 Position { get; set; }
        
        // コスト
        public float Cost { get; set; }
        
        // 推定コスト
        public float Heuristic { get; set; }
        
        // 親の座標
        public Vector2 Parent { get; set; }
        
        // 開いているか
        public bool IsOpen { get; set; }
        
        // 合計コスト
        public float Total => Cost + Heuristic;
    }

    /// <summary>
    /// A*探索
    /// </summary>
    public class AStarPath
    {
        public IMapManager Map;
        private List<CellInfo> _cellInfos;
        private bool _exitFlg;

        /// <summary>
        /// A*探索
        /// </summary>
        /// <param name="mapManager"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <returns></returns>
        public Vector2 AStarSearchPathFinding(IMapManager mapManager, Vector2Int startPos, Vector2Int endPos)
        {
            Map = mapManager;
            _cellInfos = Array.Empty<CellInfo>().ToList();
            
            Vector2 path = startPos;

            // スタート地点を設定
            var startCell = new CellInfo
            {
                Position = startPos,
                Cost = 0,
                Heuristic = Vector2.Distance(startPos, endPos),
                Parent = new Vector2Int(-999, -999), // 取り得ないもの
                IsOpen = true
            };

            _cellInfos.Add(startCell);

            _exitFlg = false;

            // ゴールに到達するまで繰り返す
            while (_cellInfos.Any(x => x.IsOpen) && !_exitFlg)
            {
                // 最小コストの開いたセルを取得
                var minCell = _cellInfos
                    .Where(x => x.IsOpen)
                    .OrderBy(x => x.Total)
                    .First();

                // 周辺マスを開く
                OpenSurroundCell(minCell, endPos, ref path);

                // 開いたセルを閉じる
                minCell.IsOpen = false;
            }

            return path;
        }

        /// <summary>
        /// 周辺マスを開く
        /// </summary>
        /// <param name="center"></param>
        /// <param name="endPos"></param>
        /// <param name="path"></param>
        private void OpenSurroundCell(CellInfo center, Vector2 endPos, ref Vector2 path)
        {
            // 上下左右のオフセットを定義
            Vector2Int[] offsets = new[]
            {
                new Vector2Int(0, 1), // 上
                new Vector2Int(0, -1), // 下
                new Vector2Int(1, 0), // 右
                new Vector2Int(-1, 0), // 左
            };

            foreach (var direction in offsets)
            {
                // 配置を取得
                Vector2Int position = new Vector2Int((int)center.Position.x, (int)center.Position.y) + direction;

                // セル情報がある場合はスキップ
                if (_cellInfos.Any(x => x.Position == position))
                {
                    continue;
                }
                
                // 通行不可の場合
                if (!Map.CanThrough(position.x, position.y))
                {
                    // スキップ
                    continue;
                }

                // 壁の場合
                CellInfo cell;
                // 敵の場合
                if(Map.IsExistEnemy(position))
                {
                    cell = CreateCellInfo(position, endPos, center, MapTile.Enemy);
                }
                // 通常の場合
                else
                {
                    cell = CreateCellInfo(position, endPos, center, MapTile.Floor);
                }
                // セル情報がない場合は新規作成
                _cellInfos.Add(cell);

                // ゴールに到達した場合は終了処理
                if (position == endPos)
                {
                    // 親の情報をたどっていく
                    CellInfo preCell = cell;
                    // 初回手前まで遡る
                    while(preCell.Parent != new Vector2Int(-999, -999))
                    {
                        // 移動先の座標を取得
                        path = preCell.Position;
                        
                        // 親を取得
                        preCell = _cellInfos.First(x => x.Position == preCell.Parent);
                    }

                    // ゴールに到達したので終了
                    _exitFlg = true;
                    return;
                }
            }
        }

        /// <summary>
        /// セル情報を作成
        /// </summary>
        /// <param name="position"></param>
        /// <param name="endPos"></param>
        /// <param name="center"></param>
        /// <param name="mapTile"></param>
        /// <returns></returns>
        private static CellInfo CreateCellInfo(
            Vector2Int position, 
            Vector2 endPos, 
            CellInfo center,
            MapTile mapTile
            )
        {
            const int PASSAGE_COST = 1;
            const int WALL_COST = 1000;
            const int ENEMY_COST = 1000;
            int additionalCost = mapTile.Properties switch
            {
                CellProperties.WALL  => WALL_COST,
                CellProperties.ENEMY => ENEMY_COST,
                _                    => PASSAGE_COST
            };
            
            return new CellInfo
            {
                Position = position,
                Cost = center.Cost + additionalCost,
                Heuristic = Vector2.Distance(position, endPos),
                Parent = center.Position,
                IsOpen = true
            };
        }
    }
}