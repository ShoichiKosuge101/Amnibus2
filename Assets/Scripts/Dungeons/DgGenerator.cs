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
        private Layer2D _layer2D = null;
        
        private readonly Stack<DgDivision> _divisions = new Stack<DgDivision>();
        
        private const int WIDTH = 20;
        private const int HEIGHT = 20;
        
        // 区画の最小サイズ
        private const int MIN_ROOM_SIZE = 5;
        // 区画の最大サイズ
        private const int MAX_SIZE = 10;
        // 区画間の最小の間隔
        private const int OUTER_MARGIN = 2;
        // 乱数範囲調整
        private const int RANDOM_RANGE_OFFSET = 1;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DgGenerator()
        {
            // 初期化
            _layer2D = new Layer2D(WIDTH, HEIGHT);
            
            // 全てを壁で埋める
            _layer2D.Fill(Chip.WALL);
            
            // マップサイズの中でまずは1つの区画を作る
            // 0を考慮しているので、-1している
            CreateDivision(0, 0, WIDTH - 1, HEIGHT - 1);
            
            // 区画を分割
            bool isVertical = Random.Range(0,2) == 0;
            SplitDivision(isVertical);
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
        private void SplitDivision(bool isVertical)
        {
            // 最後の区画を取得(分割対象)
            if (!_divisions.TryPop(out DgDivision lastDivision))
            {
                Debug.LogError("Division not found.");
                
                return;
            }
            
            var childDivision = new DgDivision();
            
            // 分割する区画を決定
            if(isVertical)
            {
                // もう分割できない場合はもとに戻して終了
                if (!CheckDivisionSize(lastDivision.Outer.Width))
                {
                    _divisions.Push(lastDivision);
                    return;
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
                    return;
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
    }
}