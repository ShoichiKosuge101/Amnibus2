using System;

namespace Constants
{
    [Flags]
    public enum CellProperties
    {
        WALL     = 0b10000000,
        GOAL     = 0b01000100,
        ENEMY    = 0b10100000,
        TREASURE = 0b00010100,
        TRAP     = 0b00001100,
        EVENT    = 0b00000100,
        NPC      = 0b10000010,
        PLAYER   = 0b10000001,
        FLOOR    = 0b00000000,
    }
    
    public record MapTile(CellProperties Properties)
    {
        public bool IsWall     => Properties.HasFlag(CellProperties.WALL);
        public bool IsGoal     => Properties.HasFlag(CellProperties.GOAL);
        public bool IsEnemy    => Properties.HasFlag(CellProperties.ENEMY);
        public bool IsTreasure => Properties.HasFlag(CellProperties.TREASURE);
        public bool IsTrap     => Properties.HasFlag(CellProperties.TRAP);
        public bool IsEvent    => Properties.HasFlag(CellProperties.EVENT);
        public bool IsNpc      => Properties.HasFlag(CellProperties.NPC);
        public bool IsPlayer   => Properties.HasFlag(CellProperties.PLAYER);
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
    }    
}

