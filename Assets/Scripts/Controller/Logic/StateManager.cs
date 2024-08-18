﻿using System;
using Constants;
using Controller.Logic.State;
using Dungeons;
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
        
        /// <summary>
        /// 現在の状態
        /// </summary>
        public StateBase CurrentState { get; private set; }
        
        /// <summary>
        /// ゴールに到達したかどうか
        /// </summary>
        public bool IsGoalReached { get; private set; }
        
        /// <summary>
        /// Map管理クラス
        /// </summary>
        public readonly IMapManager _mapManager;
        public IMapManager MapManager => _mapManager;
        
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
        /// コンストラクタ
        /// </summary>
        public StateManager(
            DungeonData dungeonData, 
            MapDisplay mapDisplay, 
            CinemachineCamera virtualCamera
            )
        {
            // 最終的なマップ情報をマップ管理クラスに渡す
            _mapManager = ServiceLocator.Instance.Resolve<IMapManager>();

            SetUpState = new SetUpState(this, dungeonData, _mapManager, mapDisplay, virtualCamera);
            PlayerInputState = new PlayerInputState(this);
            PlayerMoveState = new PlayerMoveState(this);
            PlayerAttackState = new PlayerAttackState(this);
            EnemyMoveState = new EnemyMoveState(this);
            EnemyAttackState = new EnemyAttackState(this);
            TurnEndState = new TurnEndState(this);

            // 初期状態を設定
            ChangeState(SetUpState);
            
            // イベント購読
            SubscribeEvents(_mapManager, mapDisplay);
            
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
        /// デストラクタ
        /// </summary>
        ~StateManager()
        {
            _disposable.Dispose();
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
        
        public void ChangeScene(string sceneName)
        {
            _onChangeSceneRx.OnNext(sceneName);
            
            // 終了処理
            CurrentState = null;
        }
    }
}