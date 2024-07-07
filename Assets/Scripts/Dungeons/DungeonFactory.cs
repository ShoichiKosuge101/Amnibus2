using UnityEngine;

namespace Dungeons
{
    /// <summary>
    /// ダンジョン初期化
    /// </summary>
    public class DungeonFactory
    {
        /// <summary>
        /// ダンジョン生成
        /// </summary>
        /// <returns></returns>
        public DgGenerator GenerateDungeon()
        {
            var dgGenerator = new DgGenerator();
            return dgGenerator;
        }
    }
}