using UnityEngine;

namespace Constants
{
    public record ItemKind(ItemKind.Kind kind, long ItemId)
    {
        public enum Kind
            : long
        {
            HP               = 1,
            HERB             = 2,
            HERB_TEA         = 3,
            HERB_TEA_SPECIAL = 4,
        }
        
        // オブジェクト名からアイテム種別を取得
        public static Kind GetKindFromObjectName(string objectName)
        {
            return objectName switch
            {
                "Hp" => Kind.HP,
                "Herb" => Kind.HERB,
                "Tea" => Kind.HERB_TEA,
                "TeaSpecial" => Kind.HERB_TEA_SPECIAL,
                _ => throw new System.ArgumentOutOfRangeException(nameof(objectName), objectName, null)
            };
        }
        
        /// <summary>
        /// タグ名からアイテム種別を取得
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ItemKind GetItemKind(in string objectName)
        {
            var itemKind = GetKindFromObjectName(objectName);
            return itemKind switch
            {
                Kind.HP   => Hp,
                Kind.HERB => Herb,
                Kind.HERB_TEA => HerbTea,
                Kind.HERB_TEA_SPECIAL => HerbTeaSpecial,
                _ => throw new System.ArgumentOutOfRangeException(nameof(itemKind), itemKind, null)
            };
        }
        
        public static readonly ItemKind Hp = new ItemKind(Kind.HP, (long)Kind.HP);
        public static readonly ItemKind Herb = new ItemKind(Kind.HERB, (long)Kind.HERB);
        public static readonly ItemKind HerbTea = new ItemKind(Kind.HERB_TEA, (long)Kind.HERB_TEA);
        public static readonly ItemKind HerbTeaSpecial = new ItemKind(Kind.HERB_TEA_SPECIAL, (long)Kind.HERB_TEA_SPECIAL);
    }
}