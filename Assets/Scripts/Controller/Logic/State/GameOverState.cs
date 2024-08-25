using UniRx;
using UnityEngine;

namespace Controller.Logic.State
{
    /// <summary>
    /// ゲームオーバーState
    /// </summary>
    public class GameOverState
        : StateBase
    {
        public GameOverState(in StateManager owner) : base(in owner)
        {
        }

        public override void OnEnter()
        {
            // ゲームオーバー処理
            Debug.Log("<color=red>Game Over</color>");
            
            // ゲームオーバーの通知
            Owner.OnGameOverRx.OnNext(Unit.Default);
        }
    }
}