using Constants;
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
        
        [SerializeField]
        private DungeonData dungeonData;
        
        /// <summary>
        /// マップ描画クラス
        /// </summary>
        [SerializeField]
        private MapDisplay mapDisplay;

        /// <summary>
        /// マップ管理クラス
        /// </summary>
        private IMapManager _mapManager;
        
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
            var dgGenerator = new StandardDungeonFactory().CreateDungeon(
                dungeonData.Width, 
                dungeonData.Height,
                dungeonData.MinRoomSize,
                dungeonData.MaxRoomSize,
                dungeonData.OuterMargin
                );
            
            // 最終的なマップ情報をマップ管理クラスに渡す
            _mapManager = ServiceLocator.Instance.Resolve<IMapManager>();
            // マップ管理クラスの初期化によってプレイヤー位置やゴールの決定
            _mapManager.Initialize(dgGenerator.GetLayer(), mapDisplay);
            
            // プレイヤーを取得
            var player = mapDisplay.GetPlayer();
            
            // CineMachineのターゲットをプレイヤーに設定
            var cameraInitializer = new CameraInitializer();
            cameraInitializer.SetupFollow(virtualCamera, player.gameObject);
            
            // イベント購読
            SubscribeEvents(player, _mapManager);
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
                    
                    LoadTargetScene("Title");
                });
            
            // アイテム取得時の処理
            mapManager
                .OnItemPickedUpRx
                .TakeUntilDestroy(this)
                .Subscribe(pos =>
                {
                    Debug.Log("Item Get!");

                    // オブジェクトを破棄
                    mapDisplay.ReleaseObject(pos, MapTile.Treasure);
                    
                    // アイテムを取得したらマップからアイテムを破棄
                    _mapManager.DiscardObj(pos, MapTile.Treasure);
                });
        }
    }
}