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
        public GameObject goalPrefab;

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

                    switch (dgGenerator.GetChip(x, y))
                    {
                        case Chip.FLOOR:
                        {
                            GameObject floorPrefab = floorPool.GetObject();
                            floorPrefab.transform.position = position;
                            break;
                        }
                        case Chip.WALL:
                        {
                            GameObject wallPrefab = wallPool.GetObject();
                            wallPrefab.transform.position = position;
                            break;
                        }
                        case Chip.GOAL:
                        {
                            Instantiate(goalPrefab, position, Quaternion.identity);
                            break;
                        }
                    }
                }
            }
        }
    }
}