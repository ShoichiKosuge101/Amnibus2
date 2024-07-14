using Dungeons.Interface;

namespace Dungeons.Factory
{
    /// <summary>
    /// 通常のダンジョン生成
    /// </summary>
    public class StandardDungeonFactory
        : IDungeonFactory
    {
        public DgGenerator CreateDungeon(int width, int height)
        {
            return new DgGenerator(width, height);
        }
    }
}