using UnityEngine;

namespace Helper
{
    /// <summary>
    /// ダンジョン生成テストコンポーネント
    /// </summary>
    public class TestComponent
        : MonoBehaviour
    {
        public GameObject floorPrefab;
        public GameObject wallPrefab;
        public GameObject goalPrefab;
        public GameObject playerPrefab;
        
        public int width = 20;
        public int height = 20;
        
        public int minRoomSize = 5;
        public int maxRoomSize = 10;
        public int outerMargin = 3;
    }
}