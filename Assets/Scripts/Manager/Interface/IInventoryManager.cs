using Item;

namespace Manager.Interface
{
    /// <summary>
    /// インベントリ管理クラス
    /// </summary>
    public interface IInventoryManager
    {
        /// <summary>
        /// アイテムを追加する
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        void AddItem(long itemId, int count);
        
        /// <summary>
        /// アイテムを使用する
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        void UseItem(long itemId, int count);
        
        /// <summary>
        /// アイテムを取得する
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        int GetItem(long itemId);
        
        /// <summary>
        /// アイテム一覧取得
        /// </summary>
        /// <returns></returns>
        InventoryView.ItemData[] GetInventoryItems();
        
        /// <summary>
        /// アイテムを全削除する
        /// </summary>
        void Clear();
    }
}