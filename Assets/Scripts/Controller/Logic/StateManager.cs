using System;
using Constants;
using Controller.Logic.State;
using Cysharp.Threading.Tasks;
using Dungeons;
using Dungeons.Factory;
using Manager.Interface;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;
using Utils;

namespace Controller.Logic
{
    /// <summary>
    /// State管理クラス
    /// </summary>
    public class StateManager
    {
        public SetUpState SetUpState { get; private set; }
        public PlayerInputState PlayerInputState { get; private set; }
        public PlayerMoveState PlayerMoveState { get; private set; }
        public PlayerAttackState PlayerAttackState { get; private set; }
        public EnemyMoveState EnemyMoveState { get; private set; }
        public EnemyAttackState EnemyAttackState { get; private set; }
        public TurnEndState TurnEndState { get; private set; }
        
        public StateBase CurrentState { get; private set; }
        
        /// <summary>
        /// Map管理クラス
        /// </summary>
        public IMapManager _mapManager;
        
        private CompositeDisposable _disposable = new CompositeDisposable();
        
        private readonly Subject<string> _onChangeSceneRx = new Subject<string>();
        public IObservable<string> OnGoalReachedRx => _onChangeSceneRx;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StateManager(DungeonData dungeonData, MapDisplay mapDisplay, CinemachineCamera virtualCamera)
        {
            SetUpState = new SetUpState(this);
            PlayerInputState = new PlayerInputState(this);
            PlayerMoveState = new PlayerMoveState(this);
            PlayerAttackState = new PlayerAttackState(this);
            EnemyMoveState = new EnemyMoveState(this);
            EnemyAttackState = new EnemyAttackState(this);
            TurnEndState = new TurnEndState(this);
            
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
            SubscribeEvents(player, _mapManager, mapDisplay);

            // 初期状態を設定
            ChangeState(SetUpState);
        }

        private void SubscribeEvents(PlayerController player, IMapManager mapManager, MapDisplay mapDisplay)
        {
            // プレイヤー位置を購読
            player
                .OnPositionChanged
                .Subscribe(positionChange =>
                {
                    mapManager.UpdatePlayerPosition(positionChange.Item1, positionChange.Item2);
                })
                .AddTo(_disposable);
            
            // ゴール到達時の処理
            mapManager
                .OnGoalReachedRx
                .Subscribe(_ =>
                {
                    Debug.Log("Goal Reached!");
                    
                    _onChangeSceneRx.OnNext("Title");
                })
                .AddTo(_disposable);
            
            // アイテム取得時の処理
            mapManager
                .OnItemPickedUpRx
                .Subscribe(pos =>
                {
                    Debug.Log("Item Get!");

                    // オブジェクトを破棄
                    mapDisplay.ReleaseObject(pos, MapTile.Treasure);
                    
                    // アイテムを取得したらマップからアイテムを破棄
                    _mapManager.DiscardObj(pos, MapTile.Treasure);
                })
                .AddTo(_disposable);
        }

        /// <summary>
        /// 状態の変更
        /// </summary>
        /// <param name="nextState"></param>
        public void ChangeState(StateBase nextState)
        {
            if (CurrentState != null)
            {
                CurrentState.OnExit();
            }
            
            CurrentState = nextState;
            CurrentState.OnEnter();
        }
        
        /// <summary>
        /// 状態の更新
        /// </summary>
        public void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.OnUpdate();
            }
        }
    }
}