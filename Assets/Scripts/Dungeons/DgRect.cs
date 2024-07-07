using UnityEngine;

namespace Dungeons
{
    /// <summary>
    /// 矩形管理クラス
    /// </summary>
    public class DgRect
    {
        public int Left { get; private set; }
        public int Top { get; private set; }
        public int Right { get; private set; }
        public int Bottom { get; private set; }
        
        public int Width => Right - Left;
        public int Height => Bottom - Top;
        
        public Vector2Int Position => new Vector2Int(Left, Top);
        public Vector2Int Size     => new Vector2Int(Width, Height);
        
        /// <summary>
        /// 値の設定
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public void Set(
            in int left, 
            in int top, 
            in int right, 
            in int bottom
            )
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
        
        /// <summary>
        /// 値の設定
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public void Set(in Vector2Int position, in Vector2Int size)
        {
            Set(position.x, position.y, position.x + size.x, position.y + size.y);
        }
    }
}