using Manager.Interface;
using UnityEngine;
using Utils;

namespace Item
{
    public class InventoryPresenter
        : MonoBehaviour
    {
        [SerializeField]
        private InventoryView _inventoryView;
        
        private int _selectedItemIndex;

        public void Initialize()
        {
            // 初期化
            _selectedItemIndex = 0;
            
            // 現在のアイテムデータを取得
            var inventoryItems = ServiceLocator.Instance.Resolve<IInventoryManager>().GetInventoryItems();
            // アイテムデータをViewに渡す
            _inventoryView.SetItems(inventoryItems);
            
            // 選択中のアイテムを更新
            SetSelectedFrame(_selectedItemIndex);
            
            // 購読処理
        }
        
        private void SetSelectedFrame(int selectedItemIndex)
        {
            // 選択中のアイテムを更新
            _inventoryView.SetSelectedFrame(selectedItemIndex);
        }
    }
}