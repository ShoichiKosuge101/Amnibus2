namespace Dungeons
{
    /// <summary>
    /// ダンジョン区画
    /// </summary>
    public class DgDivision
    {
        /// <summary>
        /// 外周矩形
        /// </summary>
        public DgRect Outer { get; private set; }
        
        /// <summary>
        /// 部屋矩形
        /// </summary>
        public DgRect Room { get; private set; }
        
        /// <summary>
        /// 通路矩形
        /// </summary>
        public DgRect Road { get; private set; }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DgDivision()
        {
            Outer = new DgRect();
            Room = new DgRect();
        }
        
        /// <summary>
        /// デバッグ出力
        /// </summary>
        public void Dump()
        {
            Outer.Dump();
            Room.Dump();
        }

        /// <summary>
        /// 道を作成
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public void CreateRoad(int left, int top, int right, int bottom)
        {
            Road = new DgRect();
            Road.Set(left, top, right, bottom);
        }
    }
}
