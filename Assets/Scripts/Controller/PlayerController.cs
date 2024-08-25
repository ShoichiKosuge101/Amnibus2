using UnityEngine;
using UniRx;

namespace Controller
{
    /// <summary>
    /// プレイヤーコントローラー
    /// </summary>
    public class PlayerController 
        : ActorBase
    {
        // アイテムによって回復するHP量
        private readonly int _itemRecoverHp = 10;
        
        /// <summary>
        /// 現在のHP
        /// </summary>
        public IReadOnlyReactiveProperty<int> CurrentHpRx => _currentHpRx;
        
        public void RecoverHp()
        {
            // 最大を超えるようであれば最大値にする
            _currentHpRx.Value = Mathf.Min(_currentHpRx.Value + _itemRecoverHp, MaxHp);
        }
    }
}
