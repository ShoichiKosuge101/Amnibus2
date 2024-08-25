using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Controller.Logic.State
{
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
            
            // タイトルに戻す処理
            ChangeSceneAsync().Forget();
        }
        
        private async UniTask ChangeSceneAsync()
        {
            // 2秒待機
            await UniTask.WaitForSeconds(2);
            
            // タイトルに戻す
            Owner.ChangeScene("Title");
        }
    }
}