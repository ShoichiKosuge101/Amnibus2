using Item;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// UIビュー
    /// </summary>
    public class RpgView
        : MonoBehaviour
    {
        [SerializeField]private RectTransform _gameOverUiRoot;
        [SerializeField]private RectTransform _statusUiRoot;
        [SerializeField]private InventoryPresenter _inventoryPresenter;
        [SerializeField] private TMP_Text _hpText;
        
        public void SwitchActiveGameOverUi(bool isActive)
        {
            _gameOverUiRoot.gameObject.SetActive(isActive);
        }

        public void SwitchActiveStatusUi(bool isActive)
        {
            _statusUiRoot.gameObject.SetActive(isActive);
        }
        
        public void SetHp(int hp)
        {
            // HPを表示
            const string format = "HP: {0}";
            _hpText.text = string.Format(format, hp);
        }
        
        /// <summary>
        /// インベントリUIの表示切り替え
        /// </summary>
        /// <param name="isActive"></param>
        public void SwitchActiveInventoryUi(bool isActive)
        {
            _inventoryPresenter.gameObject.SetActive(isActive);
            // インベントリUIが表示される場合は初期化
            if (isActive)
            {
                _inventoryPresenter.Initialize();
            }
        }
    }
}