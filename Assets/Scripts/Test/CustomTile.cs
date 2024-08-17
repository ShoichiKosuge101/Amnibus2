using UnityEngine.Tilemaps;

namespace Test
{
    public class CustomTile
        : Tile
    {
        public enum TileType
        {
            Floor,
            Wall,
            Enemy,
            Item01,
        }
        
        public TileType Type;
    }
}