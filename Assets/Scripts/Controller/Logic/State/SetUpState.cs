using Dungeons;
using Dungeons.Factory;
using Manager.Interface;
using Unity.Cinemachine;
using Utils;

namespace Controller.Logic.State
{
    /// <summary>
    /// セットアップ状態
    /// </summary>
    public class SetUpState
        : StateBase
    {
        private readonly DungeonData _dungeonData;
        private readonly IMapManager _mapManager;
        private readonly MapDisplay _mapDisplay;
        private readonly CinemachineCamera _virtualCamera;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="dungeonData"></param>
        /// <param name="mapManager"></param>
        /// <param name="mapDisplay"></param>
        /// <param name="virtualCamera"></param>
        public SetUpState(
            in StateManager owner, 
            in DungeonData dungeonData, 
            in IMapManager mapManager,
            in MapDisplay mapDisplay,
            in CinemachineCamera virtualCamera
            ) 
            : base(in owner)
        {
            _dungeonData = dungeonData;
            _mapManager = mapManager;
            _mapDisplay = mapDisplay;
            _virtualCamera = virtualCamera;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void OnEnter()
        {
            // シーンの初期化処理を記述
            var dgGenerator = new StandardDungeonFactory()
                .CreateDungeon(
                    _dungeonData.Width, 
                    _dungeonData.Height,
                    _dungeonData.MinRoomSize,
                    _dungeonData.MaxRoomSize,
                    _dungeonData.OuterMargin
                    );
            
            // マップ管理クラスの初期化によってプレイヤー位置やゴールの決定
            _mapManager.Initialize(dgGenerator.GetLayer(), _mapDisplay);
            
            // CineMachineのターゲットをプレイヤーに設定
            var cameraInitializer = new CameraInitializer();
            cameraInitializer.SetupFollow(_virtualCamera, _mapManager.PlayerController.gameObject);
            
            // // 初期化フラグを下す
            // Owner.SetInitializeFlag(false);
            
            Owner.ChangeState(Owner.PlayerInputState);
        }
    }
}