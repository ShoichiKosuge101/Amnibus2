namespace Dungeons.Interface
{
    /// <summary>
    /// ダンジョン生成インターフェース
    /// </summary>
    public interface IDungeonGenerator
    {
        int Width { get; set; }
        int Height { get; set; }
        
        void Generate();
        Layer2D GetLayer();
    }
}