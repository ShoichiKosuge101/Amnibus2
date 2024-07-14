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
        public IEnumerable<DgDivision> Divisions => _divisions;
        
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
            
            // 部屋同士をつなぐ
            ConnectRooms();
            
            // ゴールを設定
            PlaceGoal();
        }

        /// <summary>
        /// 部屋をつなぐ
        /// </summary>
        private void ConnectRooms()
        {
            // 接続する２つの部屋を決める(隣接する部屋は接続できる)
            for(int roomIndex = 0; roomIndex < _divisions.Count - 1; ++roomIndex)
            {
                var room1 = _divisions.ElementAt(roomIndex);
                var room2 = _divisions.ElementAt(roomIndex + 1);
                
                Debug.Log($"Connecting Room1: {room1.Outer.Left}, {room1.Outer.Top}, {room1.Outer.Right}, {room1.Outer.Bottom} with Room2: {room2.Outer.Left}, {room2.Outer.Top}, {room2.Outer.Right}, {room2.Outer.Bottom}");

                // ２つの部屋を接続する
                CreateRoad(room1, room2);
            }
        }

        /// <summary>
        /// 部屋同士をつなぐ
        /// </summary>
        /// <param name="dgDiv1"></param>
        /// <param name="dgDiv2"></param>
        private void CreateRoad(DgDivision dgDiv1, DgDivision dgDiv2)
        {
            Debug.Log($"Room1: {dgDiv1.Outer.Left}, {dgDiv1.Outer.Top}, {dgDiv1.Outer.Right}, {dgDiv1.Outer.Bottom}");
            Debug.Log($"Room2: {dgDiv2.Outer.Left}, {dgDiv2.Outer.Top}, {dgDiv2.Outer.Right}, {dgDiv2.Outer.Bottom}");
            
            // 部屋がどの面で接しているかを調べる
            if (dgDiv1.Outer.Bottom == dgDiv2.Outer.Top)
            {
                // 上下に接している場合
                CreateVerticalRoad(dgDiv1, dgDiv2);
                return;
            }
            
            if(dgDiv1.Outer.Top == dgDiv2.Outer.Bottom)
            {
                // 下上に接している場合
                CreateVerticalRoad(dgDiv2, dgDiv1);
                return;
            }

            // 左右で面している場合
            if (dgDiv1.Outer.Right == dgDiv2.Outer.Left)
            {
                // 左右に接している場合
                CreateHorizontalRoad(dgDiv1, dgDiv2);
                return;
            }
            if (dgDiv1.Outer.Left == dgDiv2.Outer.Right)
            {
                // 右左に接している場合
                CreateHorizontalRoad(dgDiv2, dgDiv1);
            }
        }

        /// <summary>
        /// 垂直方向の道を作成
        /// </summary>
        /// <param name="leftDiv"></param>
        /// <param name="rightDiv"></param>
        private void CreateHorizontalRoad(DgDivision leftDiv, DgDivision rightDiv)
        {
            // 部屋の横幅から道を作る点を決める
            int y1 = Random.Range(leftDiv.Room.Top, leftDiv.Room.Bottom);
            int y2 = Random.Range(rightDiv.Room.Top, rightDiv.Room.Bottom);
            int x = leftDiv.Outer.Right;
            
            // 部屋1から右に線を伸ばす
            leftDiv.CreateRoad(
                leftDiv.Room.Right, 
                y1, 
                x, 
                y1 + 1
            );
            
            // 部屋2から左に線を伸ばす
            rightDiv.CreateRoad(
                x, 
                y2, 
                rightDiv.Room.Left, 
                y2 + 1
            );
            
            FillRoom(leftDiv.Road);
            FillRoom(rightDiv.Road);
            
            // 道をつなぐ
            if(y1 < y2)
            {
                FillVLine(y1, y2, x);
            }
            else
            {
                FillVLine(y2, y1, x);
            }
        }
        
        /// <summary>
        /// 水平方向の道を作成
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="y"></param>
        private void FillHLine(int left, int right, int y)
        {
            Debug.Log($"Filling horizontal line from ({left}, {y}) to ({right}, {y})");
            _layer2D.FillRect4Point(left, y, right + 1, y + 1, MapTile.Floor);
        }

        /// <summary>
        /// 部屋から縦方向の道を作る
        /// </summary>
        /// <param name="topDiv"></param>
        /// <param name="bottomDiv"></param>
        private void CreateVerticalRoad(DgDivision topDiv, DgDivision bottomDiv)
        {
            // 部屋の縦幅から道を作る点を決める
            int x1 = Random.Range(topDiv.Room.Left, topDiv.Room.Right);
            int x2 = Random.Range(bottomDiv.Room.Left, bottomDiv.Room.Right);
            int y = topDiv.Outer.Bottom;
            
            // 部屋1から下に線を伸ばす
            topDiv.CreateRoad(
                x1, 
                topDiv.Room.Bottom, 
                x1 + 1, 
                y
            );
            
            // 部屋2から上に線を伸ばす
            bottomDiv.CreateRoad(
                x2, 
                y, 
                x2 + 1, 
                bottomDiv.Room.Top
            );
            
            FillRoom(topDiv.Road);
            FillRoom(bottomDiv.Road);
            
            // 道をつなぐ
            if(x1 < x2)
            {
                FillHLine(x1, x2, y);
            }
            else
            {
                FillHLine(x2, x1, y);
            }
        }
        
        /// <summary>
        /// 部屋から縦方向の道を作る
        /// </summary>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="x"></param>
        private void FillVLine(int top, int bottom, int x)
        {
            _layer2D.FillRect4Point(x, top, x + 1, bottom + 1, MapTile.Floor);
        }

        /// <summary>
        /// ゴールを配置
        /// </summary>
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
            
            // 部屋の生成位置と範囲をログ出力
            Debug.Log($"Created Room: {div.Room.Left}, {div.Room.Top}, {div.Room.Right}, {div.Room.Bottom}");
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
            
            Debug.Log($"Created Division: {division.Outer.Left}, {division.Outer.Top}, {division.Outer.Right}, {division.Outer.Bottom}");
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
        
        /// <summary>
        /// マップ情報を取得
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public MapTile GetMapTile(int x, int y)
        {
            return _layer2D.Get(x, y);
        }
    }
}