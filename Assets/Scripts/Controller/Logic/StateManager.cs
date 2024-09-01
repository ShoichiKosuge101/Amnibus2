using System;
using Constants;
using Controller.Logic.State;
using Dungeons;
using Manager.Interface;
using Manager.Service;
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
        public GameOverState GameOverState { get; private set; }
        public StateBase GoalState { get; set; }

        /// <summary>
        /// 現在の状態
        /// </summary>
        public StateBase CurrentState { get; private set; }
        
        /// <summary>
        /// ゴールに到達したかどうか
        /// </summary>
        public bool IsGoalReached { get; private set; }
        
        /// <summary>
        /// ゲームオーバーかどうか
        /// </summary>
        public bool IsGameOver { get; private set; }

        /// <summary>
        /// Map管理クラス
        /// </summary>
        public IMapManager MapManager { get; }

        /// <summary>
        /// 購読管理
        /// </summary>
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        /// <summary>
        /// シーン変更イベント
        /// </summary>
        private readonly Subject<string> _onChangeSceneRx = new Subject<string>();
        public IObservable<string> OnGoalReachedRx => _onChangeSceneRx;

        /// <summary>
        /// ゲームオーバーイベント
        /// </summary>
        public readonly Subject<Unit> OnGameOverRx = new Subject<Unit>();
        
        /// <summary>
        /// 現在のHP
        /// </summary>
        public readonly Subject<int> OnChangeHpRx = new Subject<int>();
        private readonly PlayerHpService _playerHpService;
        
        // // シーン遷移中の購読処理中断フラグ
        // private bool _isInitializing;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StateManager(
            DungeonData dungeonData, 
            MapDisplay mapDisplay, 
            CinemachineCamera virtualCamera
            )
        {
            // フラグの初期化
            IsGoalReached = false;
            IsGameOver = false;
            
            // 最終的なマップ情報をマップ管理クラスに渡す
            MapManager = ServiceLocator.Instance.Resolve<IMapManager>();
            // Hp管理サービスを取得
            _playerHpService = ServiceLocator.Instance.Resolve<PlayerHpService>();

            // ダンジョン生成に必要なものを渡す
            SetUpState = new SetUpState(
                this, 
                dungeonData, 
                MapManager, 
                mapDisplay, 
                virtualCamera);
            
            PlayerInputState = new PlayerInputState(this);
            PlayerMoveState = new PlayerMoveState(this);
            PlayerAttackState = new PlayerAttackState(this);
            EnemyMoveState = new EnemyMoveState(this);
            EnemyAttackState = new EnemyAttackState(this);
            TurnEndState = new TurnEndState(this);
            GameOverState = new GameOverState(this);
            GoalState = new GoalState(this);

            // 初期状態を設定
            ChangeState(SetUpState);
            
            // イベント購読
            SubscribeEvents(MapManager, mapDisplay);
            
            // 毎フレーム更新
            Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    Update();
                })
                .AddTo(_disposable);
        }

        /// <summary>
        /// イベント購読
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mapManager"></param>
        /// <param name="mapDisplay"></param>
        private void SubscribeEvents(
            IMapManager mapManager, 
            MapDisplay mapDisplay
            )
        {
            // プレイヤー位置を購読
            mapManager.PlayerController
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
                    
                    IsGoalReached = true;
                })
                .AddTo(_disposable);
            
            // アイテム取得時の処理
            mapManager
                .OnItemPickedUpRx
                .Subscribe(pos =>
                {
                    Debug.Log("Item Get!");
                    
                    // TODO: オブジェクト名を取得できるようにする
                    var itemKind = ItemKind.GetItemKind("Hp");
                    // // インベントリに保存
                    // mapManager.InventoryManager.AddItem(itemKind);
                    // // DEBUG: インベントリの中身を表示
                    // mapManager.InventoryManager.ShowInventory();
                    
                    // プレイヤーの回復処理
                    mapManager.PlayerController.RecoverHp();

                    // オブジェクトを破棄
                    mapDisplay.ReleaseObject(pos, MapTile.Treasure);
                    
                    // アイテムを取得したらマップからアイテムを破棄
                    MapManager.DiscardObj(pos, MapTile.Treasure);
                })
                .AddTo(_disposable);
            
            // プレイヤーのHPを購読して、UI表示に渡す
            mapManager.PlayerController
                .CurrentHpRx
                .SkipLatestValueOnSubscribe()
                .Subscribe(hp =>
                {
                    // // 初期化中は処理をスキップ
                    // if(_isInitializing)
                    // {
                    //     return;
                    // }
                    
                    // シーン遷移時の管理用サービスに現在のHPを渡す
                    _playerHpService.SetHp(hp);
                    
                    OnChangeHpRx.OnNext(hp);
                })
                .AddTo(_disposable);
            
            // プレイヤーの死亡処理
            mapManager.PlayerController
                .OnDefeatRx
                .Subscribe(_ =>
                {
                    // この購読はいらないかも
                    Debug.Log("Player Dead!");
                    
                    IsGameOver = true;
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
        
        // /// <summary>
        // /// 初期化フラグの設定
        // /// </summary>
        // /// <param name="isInitializing"></param>
        // public void SetInitializeFlag(bool isInitializing)
        // {
        //     _isInitializing = isInitializing;
        // }
        
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
        
        /// <summary>
        /// シーン変更
        /// </summary>
        /// <param name="sceneName"></param>
        public void ChangeScene(string sceneName)
        {
            _onChangeSceneRx.OnNext(sceneName);
            
            // 終了処理
            Exit();
        }
        
        /// <summary>
        /// 終了処理
        /// </summary>
        private void Exit()
        {
            CurrentState = null;
            _disposable.Dispose();
        }
    }
}