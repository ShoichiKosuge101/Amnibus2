using Controller;
using Dungeons;
using Dungeons.Factory;
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
        public CinemachineCamera virtualCamera;
        
        public int width = 20;
        public int height = 20;
        
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
            // シーンの初期化処理を記述
            var dgGenerator = new StandardDungeonFactory().CreateDungeon(width, height);
            
            // 最終的なマップ情報をマップ管理クラスに登録
            var mapManager = ServiceLocator.Instance.Resolve<IMapManager>();
            mapManager.SetMap(dgGenerator.Layer2D);
            
            // MapDisplayを取得
            var mapDisplay = GetComponent<MapDisplay>();
            mapDisplay.DisplayMap(dgGenerator);
            
            // プレイヤーを生成
            mapDisplay.SpawnPlayer(mapManager.GetRandomFloor());
            var player = mapDisplay.GetPlayer();
            
            // CineMachineのターゲットをプレイヤーに設定
            var cameraInitializer = new CameraInitializer();
            cameraInitializer.SetupFollow(virtualCamera, player.gameObject);
            
            // イベント購読
            SubscribeEvents(player, mapManager);
        }

        /// <summary>
        /// イベント購読
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mapManager"></param>
        private void SubscribeEvents(
            PlayerController player, 
            IMapManager mapManager
            )
        {
            // プレイヤー位置を購読
            player
                .OnPositionChanged
                .TakeUntilDestroy(this)
                .Subscribe(positionChange =>
                {
                    mapManager.UpdatePlayerPosition(positionChange.Item1, positionChange.Item2);
                });
            
            // ゴール到達時の処理
            mapManager
                .OnGoalReachedRx
                .TakeUntilDestroy(this)
                .Subscribe(_ =>
                {
                    Debug.Log("Goal Reached!");
                });
        }
    }
}