using Constants;
using UnityEngine;
using Utils;

namespace Dungeons
{
    /// <summary>
    /// マップ描画
    /// </summary>
    public class MapDisplay
        : MonoBehaviour
    {
        // Prefab
        public GameObject playerPrefab;

        // ObjectPool
        public ObjectPool floorPool;
        public ObjectPool wallPool;
        
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
                        GameObject floorPrefab = floorPool.GetObject();
                        floorPrefab.transform.position = position;
                    }
                    else if (dgGenerator.GetChip(x, y) == Chip.WALL)
                    {
                        GameObject wallPrefab = wallPool.GetObject();
                        wallPrefab.transform.position = position;
                    }
                }
            }
        }
    }
}