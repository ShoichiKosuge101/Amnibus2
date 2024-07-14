namespace Dungeons.Interface
{
    public interface IDungeonFactory
    {
        /// <summary>
        /// ダンジョン生成
        /// </summary>
        /// <returns></returns>
        DgGenerator CreateDungeon(int width, int height);
    }
}