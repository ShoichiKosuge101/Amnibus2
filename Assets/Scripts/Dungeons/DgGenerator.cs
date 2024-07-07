using System.Collections.Generic;
using System.Linq;
using Constants;
using UnityEngine;

namespace Dungeons
{
   /// <summary>
   /// ダンジョン生成
   /// </summary>
    public class DgGenerator
    {
        private readonly Layer2D _layer2D = null;
        public Layer2D Layer2D => _layer2D;
        
        private readonly Stack<DgDivision> _divisions = new Stack<DgDivision>();
        
        private const int WIDTH = 20;
        public int Width => WIDTH;
        private const int HEIGHT = 20;
        public int Height => HEIGHT;
        
        // 区画の最小サイズ
        private const int MIN_ROOM_SIZE = 5;
        // 区画の最大サイズ
        private const int MAX_SIZE = 10;
        // 区画間の最小の間隔
        private const int OUTER_MARGIN = 2;
        // 乱数範囲調整
        private const int RANDOM_RANGE_OFFSET = 1;
        private const int POS_MARGIN = 1;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DgGenerator()
        {
            // 初期化
            _layer2D = new Layer2D(WIDTH, HEIGHT);
            
            // 全てを壁で埋める
            _layer2D.Fill(MapTile.Wall);
            
            // マップサイズの中でまずは1つの区画を作る
            // 0を考慮しているので、-1している
            CreateDivision(0, 0, WIDTH - 1, HEIGHT - 1);
            
            // 区画を分割
            SplitAllDivision();
            
            // 区画に部屋を作る
            foreach (var division in _divisions)
            {
                CreateRoom(division);
            }
            
            // 部屋をつなぐ通路を作る
            ConnectPath();
            
            // ゴールを設定
            PlaceGoal();
            
            // マップ情報を出力
            _layer2D.Dump();
        }

        private void PlaceGoal()
        {
            List<Vector2Int> floorPositions = new List<Vector2Int>();

            // 床の位置を全部集める
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    if (_layer2D.Get(x, y) == MapTile.Floor)
                    {
                        floorPositions.Add(new Vector2Int(x, y));
                    }
                }
            }

            // プレイヤー位置を(1,1)と仮定して、それ以外なら配置可能
            Vector2Int _goalPosition = Vector2Int.one;
            while (_goalPosition == Vector2Int.one)
            {
                _goalPosition = floorPositions[Random.Range(0, floorPositions.Count)];
            }
            
            _layer2D.Set(_goalPosition.x, _goalPosition.y, MapTile.Goal);
        }

        /// <summary>
        /// 全区画を分割
        /// </summary>
        private void SplitAllDivision()
        {
            while (_divisions.Count > 0)
            {
                bool isVertical = Random.Range(0, 2) == 0;
                
                // 区画を分割
                if(!TrySplitDivision(isVertical))
                {
                    // 分割できない場合は終了
                    break;
                }
            }
        }

        /// <summary>
        /// 通路を接続
        /// </summary>
        private void ConnectPath()
        {
            // リスト化
            var divisions = _divisions.ToList();
            
            for (int i = 0; i < _divisions.Count - 1; ++i)
            {
                // リストの前後区画は必ず接続する
                var div1 = divisions[i];
                var div2 = divisions[i + 1];
                
                // 通路を作る
                CreatePath(div1, div2);
            }
        }

        /// <summary>
        /// 通路を作成
        /// </summary>
        /// <param name="div1"></param>
        /// <param name="div2"></param>
        private void CreatePath(DgDivision div1, DgDivision div2)
        {
            // 通路の開始位置を決める
            int x1 = Random.Range(div1.Room.Left + 1, div1.Room.Right - 1);
            int y1 = Random.Range(div1.Room.Top + 1, div1.Room.Bottom - 1);
            
            int x2 = Random.Range(div2.Room.Left + 1, div2.Room.Right - 1);
            int y2 = Random.Range(div2.Room.Top + 1, div2.Room.Bottom - 1);
            
            // L字型をランダムで生成
            if(Random.Range(0, 2) == 0)
            {
                // 横->縦
                CreateHorizontalPath(x1, x2, y1);
                CreateVerticalPath(x2, y1, y2);
            }
            else
            {
                // 縦->横
                CreateVerticalPath(x1, y1, y2);
                CreateHorizontalPath(x1, x2, y2);
            }
        }

        /// <summary>
        /// 縦の通路を作成
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        private void CreateVerticalPath(int x1, int y1, int y2)
        {
            int top = Mathf.Min(y1, y2);
            int bottom = Mathf.Max(y1, y2);
            
            for (int y = top; y <= bottom; ++y)
            {
                _layer2D.Set(x1, y, MapTile.Floor);
            }
        }

        /// <summary>
        /// 横の通路を作成
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        private void CreateHorizontalPath(int x1, int x2, int y1)
        {
            int left = Mathf.Min(x1, x2);
            int right = Mathf.Max(x1, x2);
            
            for (int x = left; x <= right; ++x)
            {
                _layer2D.Set(x, y1, MapTile.Floor);
            }
        }

        /// <summary>
        /// 部屋を作成
        /// </summary>
        /// <param name="div"></param>
        private void CreateRoom(DgDivision div)
        {
            // 基準サイズを決める
            int dw = div.Outer.Width - OUTER_MARGIN;
            int dh = div.Outer.Height - OUTER_MARGIN;
            
            // 大きさをランダムに決める
            int sw = Random.Range(MIN_ROOM_SIZE, dw);
            int sh = Random.Range(MIN_ROOM_SIZE, dh);
            
            // 最大サイズを超えないようにする
            sw = Mathf.Min(sw, MAX_SIZE);
            sh = Mathf.Min(sh, MAX_SIZE);
            
            // 空きサイズを計算
            int rw = dw - sw;
            int rh = dh - sh;
            
            // 部屋の左上位置を決める
            int rx = Random.Range(0, rw) + POS_MARGIN;
            int ry = Random.Range(0, rh) + POS_MARGIN;
            
            int left = div.Outer.Left + rx;
            int top = div.Outer.Top + ry;
            int right = left + sw;
            int bottom = top + sh;
            
            // 部屋の大きさを決める
            div.Room.Set(left, top, right, bottom);
            
            // 部屋を作る
            FillRoom(div.Room);
        }

        /// <summary>
        /// 部屋を作成
        /// </summary>
        /// <param name="divRoom"></param>
        private void FillRoom(in DgRect divRoom)
        {
            for (int x = divRoom.Left; x < divRoom.Right; ++x)
            {
                for (int y = divRoom.Top; y < divRoom.Bottom; ++y)
                {
                    _layer2D.Set(x, y, MapTile.Floor);
                }
            }
        }

        /// <summary>
        /// 区画を作成
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        private void CreateDivision(in int left, in int top, in int right, in int bottom)
        {
            var division = new DgDivision();
            division.Outer.Set(left, top, right, bottom);
            _divisions.Push(division);
        }
        
        /// <summary>
        /// 区画を分割
        /// </summary>
        /// <param name="isVertical"></param>
        private bool TrySplitDivision(bool isVertical)
        {
            // 最後の区画を取得(分割対象)
            if (!_divisions.TryPop(out DgDivision lastDivision))
            {
                Debug.LogError("Division not found.");
                
                return false;
            }
            
            var childDivision = new DgDivision();
            
            // 分割する区画を決定
            if(isVertical)
            {
                // もう分割できない場合はもとに戻して終了
                if (!CheckDivisionSize(lastDivision.Outer.Width))
                {
                    _divisions.Push(lastDivision);
                    return false;
                }
                
                // 分割区画の右側を使用
                // 最小のx座標と最大のx座標を取得し、その中でランダムな位置pを決定
                int min = lastDivision.Outer.Left + (MIN_ROOM_SIZE + OUTER_MARGIN);
                int max = lastDivision.Outer.Right - (MIN_ROOM_SIZE + OUTER_MARGIN);
                // 区画の最大サイズを超えないように調整
                int distance = max - min;
                distance = Mathf.Min(distance, MAX_SIZE);
                // 分割位置を決定
                // minからの距離distanceまでのランダムな位置pを決定
                int p = min + Random.Range(0, distance + RANDOM_RANGE_OFFSET); // distance + 1 にすることで、maxも含める
                
                // 分割位置で区画を横に分割
                childDivision.Outer.Set(
                    lastDivision.Outer.Left, 
                    lastDivision.Outer.Top, 
                    p, 
                    lastDivision.Outer.Bottom
                    );
                
                // 最後の区画を更新
                lastDivision.Outer.Set(
                    p, 
                    lastDivision.Outer.Top, 
                    lastDivision.Outer.Right, 
                    lastDivision.Outer.Bottom
                    );
            }
            else
            {
                // もう分割できない場合はもとに戻して終了
                if (!CheckDivisionSize(lastDivision.Outer.Height))
                {
                    _divisions.Push(lastDivision);
                    return false;
                }
                
                // 分割区画の下側を使用
                // 最小のy座標と最大のy座標を取得し、その中でランダムな位置pを決定
                int min = lastDivision.Outer.Top + (MIN_ROOM_SIZE + OUTER_MARGIN);
                int max = lastDivision.Outer.Bottom - (MIN_ROOM_SIZE + OUTER_MARGIN);
                // 区画の最大サイズを超えないように調整
                int distance = max - min;
                distance = Mathf.Min(distance, MAX_SIZE);
                // 分割位置を決定
                // minからの距離distanceまでのランダムな位置pを決定
                int p = min + Random.Range(0, distance + RANDOM_RANGE_OFFSET);
                
                // 分割位置で区画を分割
                childDivision.Outer.Set(
                    lastDivision.Outer.Left, 
                    lastDivision.Outer.Top, 
                    lastDivision.Outer.Right, 
                    p
                    );
                
                // 最後の区画を更新
                lastDivision.Outer.Set(
                    lastDivision.Outer.Left, 
                    p, 
                    lastDivision.Outer.Right, 
                    lastDivision.Outer.Bottom
                    );
            }
            Debug.Log($"Child: {childDivision.Outer.Left}, {childDivision.Outer.Top}, {childDivision.Outer.Right}, {childDivision.Outer.Bottom}");
            Debug.Log($"Last: {lastDivision.Outer.Left}, {lastDivision.Outer.Top}, {lastDivision.Outer.Right}, {lastDivision.Outer.Bottom}");

            // 最後に分割した区画を追加
            // 追加順が次の分割に関わるので、ランダムな順番で追加
            if (Random.Range(0, 2) == 0)
            {
                _divisions.Push(childDivision);
                _divisions.Push(lastDivision);
            }
            else
            {
                _divisions.Push(lastDivision);
                _divisions.Push(childDivision);
            }
            
            return true;
        }

        /// <summary>
        /// 区画のサイズをチェック
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private static bool CheckDivisionSize(in int size)
        {
            // 2分割した区画のサイズが最小サイズを下回る場合は分割しない
            // すなわち、２倍してかつ接続点の道を作るための最低限のサイズ(最小区画サイズ + 外周マージン + 分割線の幅)を確保できるか判定
            return size >= 2 * (MIN_ROOM_SIZE + OUTER_MARGIN) + 1;
        }
        
        public MapTile GetMapTile(int x, int y)
        {
            return _layer2D.Get(x, y);
        }
    }
}