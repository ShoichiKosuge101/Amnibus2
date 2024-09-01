using Scene;
using UnityEngine.SceneManagement;

namespace Controller.Logic.State
{
    public class GoalState
        : StateBase
    {
        public GoalState(in StateManager owner) : base(in owner)
        {
        }

        public override void OnEnter()
        {
            // 現在のシーン名を取得
            string sceneName = SceneManager.GetActiveScene().name;
            var currentScene = StageManager.GetStage(sceneName);
            // 次のシーンを取得
            string nextSceneName = StageManager.GetNextSceneName(currentScene);
            
            // // 初期化中フラグを立てる
            // Owner.SetInitializeFlag(true);
                
            // ゴールしていれば、ゲームクリア
            Owner.ChangeScene(nextSceneName);
        }
    }
}