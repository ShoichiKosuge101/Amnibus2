using UnityEngine;

namespace Controller.Logic.State
{
    /// <summary>
    /// 状態の基底クラス
    /// </summary>
    public abstract class StateBase
    {
        /// <summary>
        /// State管理クラス
        /// </summary>
        public StateManager Owner { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="owner"></param>
        public StateBase(in StateManager owner)
        {
            Owner = owner;
        }
        
        /// <summary>
        /// 状態の初期化
        /// </summary>
        public virtual void OnEnter()
        {
            Debug.Log("StateBase.OnEnter");
        }
        
        /// <summary>
        /// 状態の更新
        /// </summary>
        public virtual void OnUpdate()
        {
            Debug.Log("StateBase.OnUpdate");
        }
        
        /// <summary>
        /// 状態の終了
        /// </summary>
        public virtual void OnExit()
        {
            Debug.Log("StateBase.OnExit");
        }
    }
}