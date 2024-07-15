using UnityEngine;

namespace Dungeons
{
    [CreateAssetMenu(fileName = "DungeonData", menuName = "Scriptable Objects/DungeonData")]
    public class DungeonData 
        : ScriptableObject
    {
        /// <summary>
        /// ダンジョンの幅
        /// </summary>
        [SerializeField] 
        private int width = 10;
        public int Width => width;

        /// <summary>
        /// ダンジョンの高さ
        /// </summary>
        [SerializeField] 
        private int height = 10;
        public int Height => height;

        [SerializeField] 
        private int minRoomSize = 5;
        public int MinRoomSize => minRoomSize;
        
        [SerializeField] 
        private int maxRoomSize = 10;
        public int MaxRoomSize => maxRoomSize;
        
        [SerializeField] 
        private int outerMargin = 3;
        public int OuterMargin => outerMargin;
    }
}
