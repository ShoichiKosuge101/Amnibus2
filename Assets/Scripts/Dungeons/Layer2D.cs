using System.Collections.Generic;
using Constants;
using UnityEngine;

namespace Dungeons
{
    /// <summary>
    /// マップ情報保持
    /// [0,1]は壁、[1,1]は通路
    /// </summary>
    public class Layer2D
    {
        private int _width;
        public int Width => _width;
        
        private int _height;
        public int Height => _height;
        
        private MapTile[,] _values;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Layer2D(in int width, in int height)
        {
            // マップ情報を生成
            Create(width, height);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="size"></param>
        public Layer2D(in Vector2Int size)
            : this(size.x, size.y)
        {
        }
        
        /// <summary>
        /// マップ情報を生成
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Create(in int width, in int height)
        {
            _width = width;
            _height = height;
            _values = new MapTile[width, height];
        }
        
        // 領域外かどうかを判定
        private bool IsOutOfRange(in int x, in int y)
        {
            return x < 0 
                   || x >= _width 
                   || y < 0 
                   || y >= _height;
        }
        
        // 値の取得
        public MapTile Get(in int x, in int y)
        {
            return !IsOutOfRange(x, y) 
                ? _values[x, y] 
                : MapTile.Floor;
        }
        
        // 値の取得
        public MapTile Get(in Vector2Int position)
        {
            return Get(position.x, position.y);
        }
        
        // 値の設定
        public void Set(in int x, in int y, in MapTile value)
        {
            if (!IsOutOfRange(x, y))
            {
                _values[x, y] = value;
            }
        }
        
        // 値の上書き
        public void RideOn(in int x, in int y, in MapTile value)
        {
            if (!IsOutOfRange(x, y))
            {
                // 値の上書き
                _values[x, y] = _values[x, y].Add(value.Properties);
            }
        }
        
        // 値の削除
        public void Remove(in int x, in int y, in MapTile value)
        {
            if (!IsOutOfRange(x, y))
            {
                // 値の削除
                _values[x, y] = _values[x, y].Remove(value.Properties);
            }
        }
        
        /// <summary>
        /// 特定の値で埋める
        /// </summary>
        /// <param name="value"></param>
        public void Fill(in MapTile value)
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    Set(x, y, value);
                }
            }
        }
        
        // 矩形領域を特定の値で埋める
        public void FillRect(
            in int x, 
            in int y, 
            in int width, 
            in int height, 
            in MapTile value
            )
        {
            // iがxからx+widthまで
            for (var i = x; i < x + width; i++)
            {
                // jがyからy+heightまで
                for (var j = y; j < y + height; j++)
                {
                    Set(i, j, value);
                }
            }
        }
        
        // 矩形領域を4点で指定して特定の値で埋める
        public void FillRect4Point(
            in int left, 
            in int top, 
            in int right, 
            in int bottom, 
            in MapTile value
            )
        {
            Debug.Log($"Filling rectangle from ({left}, {top}) to ({right}, {bottom})");
            FillRect(left, top, right - left, bottom - top, value);
        }
        
        // 矩形領域を特定の値で埋める
        public void FillRect(
            in Vector2Int position, 
            in Vector2Int size, 
            in MapTile value
            )
        {
            FillRect(position.x, position.y, size.x, size.y, value);
        }
        
        /// <summary>
        /// デバッグ出力
        /// </summary>
        public void Dump()
        {
            for (var y = 0; y < _height; y++)
            {
                var line = "";
                for (var x = 0; x < _width; x++)
                {
                    line += Get(x, y) == MapTile.Wall ? "■" : "□";
                }
                Debug.Log(line);
            }
        }
        
        /// <summary>
        /// 床の位置を全部集める
        /// </summary>
        /// <returns></returns>
        public List<Vector2Int> GetFloors()
        {
            List<Vector2Int> floorPositions = new List<Vector2Int>();

            // 床の位置を全部集める
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    if (Get(x, y).IsFloor)
                    {
                        floorPositions.Add(new Vector2Int(x, y));
                    }
                }
            }

            return floorPositions;
        }
    }
}
