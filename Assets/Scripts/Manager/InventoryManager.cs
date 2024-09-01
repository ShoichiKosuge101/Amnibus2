using System.Collections.Generic;
using Item;
using Manager.Interface;

namespace Manager
{
    /// <summary>
    /// インベントリ管理クラス
    /// </summary>
    public class InventoryManager
        : IInventoryManager
    {
        private readonly Dictionary<long, int> _inventory = new Dictionary<long, int>();
        
        /// <summary>
        /// アイテムを追加
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        public void AddItem(long itemId, int count)
        {
            if (!_inventory.TryAdd(itemId, count))
            {
                _inventory[itemId] += count;
            }
        }
        
        /// <summary>
        /// アイテムを使用
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        public void UseItem(long itemId, int count)
        {
            if (_inventory.ContainsKey(itemId))
            {
                _inventory[itemId] -= count;
                if (_inventory[itemId] <= 0)
                {
                    _inventory.Remove(itemId);
                }
            }
        }
        
        /// <summary>
        /// アイテムを取得
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public int GetItem(long itemId)
        {
            return _inventory.GetValueOrDefault(itemId, 0);
        }

        public InventoryView.ItemData[] GetInventoryItems()
        {
            var items = new List<InventoryView.ItemData>();
            foreach (var item in _inventory)
            {
                // TODO: 本当はアイテムごとのSpriteを取るようにしたい
                items.Add(new InventoryView.ItemData(null, item.Value));
            }

            return items.ToArray();
        }

        /// <summary>
        /// アイテム全削除
        /// </summary>
        public void Clear()
        {
            _inventory.Clear();
        }
    }
}