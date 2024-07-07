using Constants;
using UnityEngine;

namespace Dungeons
{
    public class MapDisplay
        : MonoBehaviour
    {
        // Prefab
        public GameObject playerPrefab;
        public GameObject floorPrefab;
        public GameObject wallPrefab;
        
        private GameObject _player;
        
        /// <summary>
        /// プレイヤーの生成
        /// </summary>
        /// <param name="position"></param>
        public void SpawnPlayer(in Vector2 position)
        {
            _player = Instantiate(playerPrefab, position, Quaternion.identity);
        }
        
        /// <summary>
        /// プレイヤーを取得
        /// </summary>
        /// <returns></returns>
        public GameObject GetPlayer()
        {
            return _player;
        }
        
        /// <summary>
        /// マップを表示
        /// </summary>
        public void DisplayMap(DgGenerator dgGenerator)
        {
            int width = dgGenerator.Width;
            int height = dgGenerator.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 position = new Vector2(x, y);

                    if (dgGenerator.GetChip(x, y) == Chip.FLOOR)
                    {
                        Instantiate(floorPrefab, position, Quaternion.identity);
                    }
                    else if (dgGenerator.GetChip(x, y) == Chip.WALL)
                    {
                        Instantiate(wallPrefab, position, Quaternion.identity);
                    }
                }
            }
        }
    }
}