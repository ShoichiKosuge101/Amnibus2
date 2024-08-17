using Scene.Common;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Interface;

namespace Scene
{
    /// <summary>
    /// タイトルシーン
    /// </summary>
    public class TitleScene
        : SceneBase
    {
        [SerializeField]
        private Button startButton;
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        protected override void InitializeScene()
        {
            // ボタンのクリックイベントを登録
            startButton.onClick
                .AsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ =>
                {
                    // シーン遷移
                    LoadTargetScene("Stage1");
                });
        }

        /// <summary>
        /// サービス登録
        /// </summary>
        protected override void RegisterSceneServices()
        {
            // 入力マネージャの登録
            ServiceLocator.Instance.Register<IInputManager>(new InputManager());
        }
    }
}