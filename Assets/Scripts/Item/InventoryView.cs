using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Item
{
    public class InventoryView
        : MonoBehaviour
    {
        [SerializeField]
        private GridLayoutGroup _gridLayout;
        
        [SerializeField]
        private ItemView _itemViewPrefab;
        
        private readonly List<ItemView> _itemViews = new List<ItemView>();
        
        /// <summary>
        /// アイテムデータ
        /// </summary>
        public class ItemData
        {
            public Sprite Sprite { get; }
            public int Count { get; }

            public ItemData(Sprite sprite, int count)
            {
                Sprite = sprite;
                Count = count;
            }
        }
        
        /// <summary>
        /// インベントリの表示作成
        /// </summary>
        /// <param name="items"></param>
        public void SetItems(ItemData[] items)
        {
            // 既存のアイテムビューを削除
            foreach (Transform child in _gridLayout.transform)
            {
                Destroy(child.gameObject);
            }
            
            // リストの初期化
            _itemViews.Clear();
            
            // アイテムビューを生成
            foreach (var item in items)
            {
                var itemView = Instantiate(_itemViewPrefab, _gridLayout.transform);
                itemView.SetItemView(item.Sprite, item.Count);
                
                _itemViews.Add(itemView);
            }
        }

        public void SetSelectedFrame(int selectedItemIndex)
        {
            // 選択中のアイテムを更新
            for (var i = 0; i < _itemViews.Count; i++)
            {
                _itemViews[i].SetSelectedFrame(i == selectedItemIndex);
            }
        }
    }
}