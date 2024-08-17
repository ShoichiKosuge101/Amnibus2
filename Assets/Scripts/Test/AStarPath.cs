using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
        
        // 親の座標
        public Vector3 Parent { get; set; }
        
        // 開いているか
        public bool IsOpen { get; set; }
        
        // 合計コスト
        public float Total => Cost + Heuristic;
    }
    
    /// <summary>
    /// A*探索
    /// </summary>
    public class AStarPath
        : MonoBehaviour
    {
        public Tilemap Map;
        public TileBase ReplaceTile;
        public GameObject Player;
        public GameObject Enemy;
        private List<CellInfo> _cellInfos = new();
        private Vector3 _goalPosition;
        private bool _exitFlg;

        private void Start()
        {
            // 初期化
            InitializeAsync().Forget();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private async UniTask InitializeAsync()
        {
            _cellInfos = new List<CellInfo>();
            
            // A*探索
            AStarSearchPathFinding();
        }

        /// <summary>
        /// A*探索
        /// </summary>
        private void AStarSearchPathFinding()
        {
            // ゴールはプレイヤーの位置
            _goalPosition = Player.transform.position;
            
            // スタート地点を設定
            var startCell = new CellInfo
            {
                Position = Enemy.transform.position,
                Cost = 0,
                Heuristic = Vector2.Distance(Enemy.transform.position, _goalPosition),
                Parent = Vector3.negativeInfinity, // 親がないので無限大
                IsOpen = true
            };
            
            _cellInfos.Add(startCell);
            
            _exitFlg = false;
            
            // ゴールに到達するまで繰り返す
            while (_cellInfos.Any(x => x.IsOpen) && !_exitFlg)
            {
                // 最小コストのセルを取得
                var minCell = _cellInfos
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
            
            // 上下左右のオフセットを定義
            Vector3Int[] offsets = new[]
            {
                new Vector3Int(0, 1, 0), // 上
                new Vector3Int(0, -1, 0), // 下
                new Vector3Int(1, 0, 0), // 右
                new Vector3Int(-1, 0, 0), // 左
            };

            foreach (var direction in offsets)
            {
                // 配置を取得
                var position = centerPosition + direction;
                // タイルを取得
                var tile = Map.GetTile(position);
                        
                // タイルがない場合はスキップ
                if (tile == null)
                {
                    continue;
                }
                    
                // セル情報を取得
                var cell = _cellInfos
                    .FirstOrDefault(x => x.Position == Map.CellToWorld(position));
                    
                // セル情報がある場合はスキップ
                if (cell != null)
                {
                    continue;
                }

                // セル情報がない場合は新規作成
                // 壁判定を併せて行う
                cell = CreateCellInfo(tile, Map.CellToWorld(position), center);
                _cellInfos.Add(cell);
                            
                // ゴールに到達した場合は終了処理
                if (Map.WorldToCell(position) == Map.WorldToCell(_goalPosition))
                {
                    // タイルを置き換え
                    CellInfo preCell = cell;
                    while (preCell != null && preCell.Parent != Vector3.negativeInfinity)
                    {
                        Map.SetTile(Map.WorldToCell(preCell.Position), ReplaceTile);
                        preCell = _cellInfos.FirstOrDefault(x => x.Position == preCell.Parent);
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
        /// <param name="tileBase"></param>
        /// <param name="position"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        private CellInfo CreateCellInfo(TileBase tileBase, Vector3 position, CellInfo center)
        {
            const int WALL_COST = 999;
            const int PASSAGE_COST = 1;
            
            var tile = tileBase as Tile;
            if (IsWall(tile))
            {
                return new CellInfo
                {
                    Position = position,
                    Cost = center.Cost + WALL_COST,
                    Heuristic = Vector2.Distance(position, _goalPosition),
                    Parent = center.Position,
                    IsOpen = true
                };
            }
            
            return new CellInfo
            {
                Position = position,
                Cost = center.Cost + PASSAGE_COST,
                Heuristic = Vector2.Distance(position, _goalPosition),
                Parent = center.Position,
                IsOpen = true
            };
        }
        
        /// <summary>
        /// 壁判定
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        bool IsWall(Tile tile)
        {
            return tile != null && tile.sprite.name.Contains("Wall");
        }
    }
}