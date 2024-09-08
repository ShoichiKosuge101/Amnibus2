using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Item
{
    /// <summary>
    /// アイテムビュー
    /// </summary>
    public class ItemView
        : MonoBehaviour
    {
        [SerializeField]
        private Image itemImage;
        
        [SerializeField]
        private TMP_Text itemCountText;
        
        [SerializeField]
        private Image selectedFrame;

        /// <summary>
        /// アイテムビューの設定
        /// </summary>
        /// <param name="itemSprite"></param>
        /// <param name="itemCount"></param>
        public void SetItemView(Sprite itemSprite, int itemCount)
        {
            // itemImage.sprite = itemSprite;
            itemCountText.text = $"x{itemCount}";
        }

        /// <summary>
        /// 選択フレームの設定
        /// </summary>
        /// <param name="isSelected"></param>
        public void SetSelectedFrame(bool isSelected)
        {
            selectedFrame.gameObject.SetActive(isSelected);
        }
    }
}