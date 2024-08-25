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
    }
}