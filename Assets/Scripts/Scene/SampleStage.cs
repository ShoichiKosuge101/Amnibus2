using Controller.Logic;
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
        }
    }
}