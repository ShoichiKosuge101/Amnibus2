using Controller.Logic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Dungeons;
using Manager;
using Manager.Interface;
using Scene.Common;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;
using Utils;
using Utils.Interface;

namespace Scene
{
    /// <summary>
    /// サンプルステージ
    /// </summary>
    public class SampleStage
        : SceneBase
    {
        [SerializeField]
        private CinemachineCamera virtualCamera;
        
        [SerializeField]
        private DungeonData dungeonData;
        
        /// <summary>
        /// マップ描画クラス
        /// </summary>
        [SerializeField]
        private MapDisplay mapDisplay;
        
        /// <summary>
        /// GameOver UI
        /// </summary>
        [SerializeField]
        private RectTransform _gameOverUiRoot;
        
        /// <summary>
        /// シーンのサービスを登録
        /// </summary>
        protected override void RegisterSceneServices()
        {
            // 入力マネージャーを登録
            ServiceLocator.Instance.Register<IInputManager>(new InputManager());
            // マップ管理クラスを登録
            ServiceLocator.Instance.Register<IMapManager>(new MapManager());
        }
        
        /// <summary>
        /// シーン初期化処理
        /// </summary>
        protected override void InitializeScene()
        {
            // State管理クラスの取得
            var stateManager = new StateManager(dungeonData, mapDisplay, virtualCamera);
            
            // UI初期化
            _gameOverUiRoot.gameObject.SetActive(false);
            
            // イベント購読
            SubscribeEvents(stateManager);
        }

        /// <summary>
        /// イベント購読
        /// </summary>
        /// <param name="stateManager"></param>
        private void SubscribeEvents(StateManager stateManager)
        {
            stateManager
                .OnGoalReachedRx
                .TakeUntilDestroy(this)
                .Subscribe(LoadTargetScene);
            
            // ゲームオーバー処理
            stateManager
                .OnGameOverRx
                .TakeUntilDestroy(this)
                .ToUniTaskAsyncEnumerable()
                .SubscribeAwait(async _ =>
                {
                    // UI表示
                    _gameOverUiRoot.gameObject.SetActive(true);
                    
                    // ２秒待って
                    await UniTask.WaitForSeconds(2);
                    
                    // タイトルへ
                    LoadTargetScene("Title");
                });
        }
    }
}