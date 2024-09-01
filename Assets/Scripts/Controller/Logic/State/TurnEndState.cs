using System.Linq;
using Cysharp.Threading.Tasks;
using Scene;
using UnityEngine.SceneManagement;

namespace Controller.Logic.State
{
    public class TurnEndState
        : StateBase
    {
        public TurnEndState(in StateManager owner) : base(in owner)
        {
        }

        public override void OnEnter()
        {
            InitializeAsync().Forget();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private async UniTask InitializeAsync()
        {
            // 全ての行動が終わるまで待つ
            await UniTask.WaitWhile(() => Owner.MapManager.EnemyManager.Enemies.Any(enemy => enemy.IsMoving));
            
            // プレイヤーが死亡していれば、ゲームオーバーStateに遷移
            if (Owner.IsGameOver)
            {
                // プレイヤーはここで削除
                Owner.MapManager.PlayerController.Delete();
                
                Owner.ChangeState(Owner.GameOverState);
                return;
            }
            
            // ゴールしていなければ、次のターンへ
            if (!Owner.IsGoalReached)
            {
                Owner.ChangeState(Owner.PlayerInputState);
            }
            else
            {
                // 現在のシーン名を取得
                string sceneName = SceneManager.GetActiveScene().name;
                var currentScene = StageManager.GetStage(sceneName);
                // 次のシーンを取得
                string nextSceneName = StageManager.GetNextSceneName(currentScene);
                
                // ゴールしていれば、ゲームクリア
                Owner.ChangeScene(nextSceneName);
            }
        }
    }
}