using Dungeons.Interface;

namespace Dungeons.Factory
{
    /// <summary>
    /// 通常のダンジョン生成
    /// </summary>
    public class StandardDungeonFactory
        : IDungeonFactory
    {
        public IDungeonGenerator CreateDungeon(
            int width, 
            int height, 
            int minRoomSize = 5, 
            int maxRoomSize = 10, 
            int outerMargin = 2
            )
        {
            return new DgGenerator(width, height, minRoomSize, maxRoomSize, outerMargin);
        }
    }
}