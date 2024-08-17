using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Test
{
    /// <summary>
    /// マス情報
    /// </summary>
    public class CellInfo
    {
        // マスの座標
        public Vector3 Position { get; set; }
        
        // コスト
        public float Cost { get; set; }
        
        // 推定コスト
        public float Heuristic { get; set; }
        
        // 合計コスト
        public float Total { get; set; }
        
        // 親の座標
        public Vector3 Parent { get; set; }
        
        // 開いているか
        public bool IsOpen { get; set; }
    }
    
    public class AStarPath
        : MonoBehaviour
    {
        public Tilemap Map;
        public TileBase ReplaceTile;
        public GameObject Player;
        public GameObject Enemy;
        private List<CellInfo> CellInfos = new();
        private Vector3 GoalPosition;
        private bool ExitFlg;

        private void Start()
        {
            CellInfos = new List<CellInfo>();
            
            // A*探索
            AStarSearchPathFinding();
        }

        /// <summary>
        /// A*探索
        /// </summary>
        public void AStarSearchPathFinding()
        {
            // ゴールはプレイヤーの位置
            GoalPosition = Player.transform.position;
            
            // スタート地点を設定
            var startCell = new CellInfo
            {
                Position = Enemy.transform.position,
                Cost = 0,
                Heuristic = Vector2.Distance(Enemy.transform.position, GoalPosition),
                Total = Vector2.Distance(Enemy.transform.position, GoalPosition),
                Parent = Vector3.negativeInfinity, // 親がないので無限大
                IsOpen = true
            };
            
            CellInfos.Add(startCell);
            
            ExitFlg = false;
            
            // ゴールに到達するまで繰り返す
            while (CellInfos.Any(x => x.IsOpen) && !ExitFlg)
            {
                // 最小コストのセルを取得
                var minCell = CellInfos
                    .Where(x => x.IsOpen)
                    .OrderBy(x => x.Total)
                    .First();
                
                // 周辺マスを開く
                OpenSurroundCell(minCell);
                
                // 開いたセルを閉じる
                minCell.IsOpen = false;
            }
        }

        /// <summary>
        /// 周辺マスを開く
        /// </summary>
        /// <param name="center"></param>
        private void OpenSurroundCell(CellInfo center)
        {
            // Vector3Int変換
            var centerPosition = Map.WorldToCell(center.Position);
            
            // 上下左右のマスをforで表現
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    // 上下左右のみ可とする
                    if (((i == 0 || j != 0) && (i != 0 || j == 0))
                        || i == 0 && j == 0)
                    {
                        continue;
                    }
                    
                    // 配置を取得
                    var position = centerPosition + new Vector3Int(i, j, 0);
                    // タイルを取得
                    var tile = Map.GetTile(position);
                        
                    // タイルがない場合はスキップ
                    if (tile == null)
                    {
                        continue;
                    }
                        
                    // セル情報を取得
                    var cell = CellInfos
                        .FirstOrDefault(x => x.Position == Map.CellToWorld(position));
                    
                    // セル情報がある場合はスキップ
                    if (cell != null)
                    {
                        continue;
                    }
                    
                    // セル情報がない場合は新規作成
                    cell = new CellInfo
                    {
                        Position = Map.CellToWorld(position),
                        Cost = center.Cost + 1,
                        Heuristic = Vector2.Distance(Map.CellToWorld(position), GoalPosition),
                        Total = center.Cost + 1 + Vector2.Distance(Map.CellToWorld(position), GoalPosition),
                        Parent = center.Position,
                        IsOpen = true
                    };
                    CellInfos.Add(cell);
                            
                    // ゴールに到達した場合は終了処理
                    if (Map.WorldToCell(position) == Map.WorldToCell(GoalPosition))
                    {
                        // タイルを置き換え
                        CellInfo preCell = cell;
                        while (preCell != null && preCell.Parent != Vector3.negativeInfinity)
                        {
                            Map.SetTile(Map.WorldToCell(preCell.Position), ReplaceTile);
                            preCell = CellInfos.FirstOrDefault(x => x.Position == preCell.Parent);
                        }
                                
                        // ゴールに到達したので終了
                        ExitFlg = true;
                        return;
                    }
                }
            }
        }
    }
}