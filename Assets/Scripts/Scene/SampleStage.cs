using Dungeons;
using Scene.Common;
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
        
        /// <summary>
        /// シーンのサービスを登録
        /// </summary>
        protected override void RegisterSceneServices()
        {
            // 入力マネージャーを登録
            ServiceLocator.Instance.Register<IInputManager>(new InputManager());
        }
        
        /// <summary>
        /// シーン初期化処理
        /// </summary>
        protected override void InitializeScene()
        {
            // シーンの初期化処理を記述
            var dgInitializer = new DungeonFactory();
            var dgGenerator = dgInitializer.GenerateDungeon();
            
            // MapDisplayを取得
            var mapDisplay = GetComponent<MapDisplay>();
            mapDisplay.DisplayMap(dgGenerator);
            
            // プレイヤーを生成
            mapDisplay.SpawnPlayer(new Vector2(0,0));
            var player = mapDisplay.GetPlayer();
            
            // Cinemachineのターゲットをプレイヤーに設定
            var cameraInitializer = new CameraInitializer();
            cameraInitializer.SetupFollow(virtualCamera, player);
        }
    }
}