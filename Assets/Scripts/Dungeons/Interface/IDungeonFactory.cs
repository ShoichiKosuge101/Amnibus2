namespace Dungeons.Interface
{
    public interface IDungeonFactory
    {
        /// <summary>
        /// ダンジョン生成
        /// </summary>
        /// <returns></returns>
        IDungeonGenerator CreateDungeon(int width, int height, int minRoomSize, int maxRoomSize, int outerMargin);
    }
}