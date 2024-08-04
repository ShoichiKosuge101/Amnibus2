using System;

namespace Constants
{
    /// <summary>
    /// マップ形状
    /// </summary>
    [Flags]
    public enum CellProperties
    {
        FLOOR    = 0b00000000,
        WALL     = 0b00000001,
        GOAL     = 0b00000010,
        ENEMY    = 0b00000100,
        TREASURE = 0b00001000,
        TRAP     = 0b00010000,
        EVENT    = 0b00100000,
        NPC      = 0b01000000,
        PLAYER   = 0b10000000,
    }

    /// <summary>
    /// マップチップ
    /// </summary>
    /// <param name="Properties"></param>
    public record MapTile(CellProperties Properties)
    {
        public bool IsWall     => (Properties & CellProperties.WALL) == CellProperties.WALL;
        public bool IsGoal     => (Properties & CellProperties.GOAL) == CellProperties.GOAL;
        public bool IsEnemy    => (Properties & CellProperties.ENEMY) == CellProperties.ENEMY;
        public bool IsTreasure => (Properties & CellProperties.TREASURE) == CellProperties.TREASURE;
        public bool IsTrap     => (Properties & CellProperties.TRAP) == CellProperties.TRAP;
        public bool IsEvent    => (Properties & CellProperties.EVENT) == CellProperties.EVENT;
        public bool IsNpc      => (Properties & CellProperties.NPC) == CellProperties.NPC;
        public bool IsPlayer   => (Properties & CellProperties.PLAYER) == CellProperties.PLAYER;
        public bool IsFloor    => Properties == CellProperties.FLOOR;

        public static readonly MapTile Wall     = new(CellProperties.WALL);
        public static readonly MapTile Goal     = new(CellProperties.GOAL);
        public static readonly MapTile Enemy    = new(CellProperties.ENEMY);
        public static readonly MapTile Treasure = new(CellProperties.TREASURE);
        public static readonly MapTile Trap     = new(CellProperties.TRAP);
        public static readonly MapTile Event    = new(CellProperties.EVENT);
        public static readonly MapTile Npc      = new(CellProperties.NPC);
        public static readonly MapTile Player   = new(CellProperties.PLAYER);
        public static readonly MapTile Floor    = new(CellProperties.FLOOR);

        // フラグを追加
        public MapTile Add(CellProperties properties)
        {
            if (Properties == CellProperties.FLOOR)
            {
                return new MapTile(properties);
            }
            return new MapTile(Properties | properties);
        }
        
        // フラグを削除
        public MapTile Remove(CellProperties properties)
        {
            return new MapTile(Properties & ~properties);
        }

        /// <summary>
        /// タグを取得
        /// </summary>
        /// <returns></returns>
        public string GetTag()
        {
            // 敵の場合
            return Properties switch
            {
                CellProperties.WALL     => "Wall",
                CellProperties.GOAL     => "Goal",
                CellProperties.ENEMY    => "Enemy",
                CellProperties.TREASURE => "Item",
                CellProperties.TRAP     => "Trap",
                CellProperties.EVENT    => "Event",
                CellProperties.NPC      => "Npc",
                CellProperties.PLAYER   => "Player",
                _                       => "Floor",
            };
        }
    }
}

