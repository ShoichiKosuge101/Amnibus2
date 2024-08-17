using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene.Common
{
    /// <summary>
    /// シーン基底クラス
    /// </summary>
    public class SceneBase
        : MonoBehaviour
    {
        /// <summary>
        /// シーン開始時に呼ばれる
        /// </summary>
        protected virtual void Awake()
        {
            // シーンのサービスを登録
            RegisterSceneServices();
        }

        /// <summary>
        /// BaseによってStartは占有する
        /// </summary>
        private void Start()
        {
            // シーンの初期化処理
            InitializeScene();
        }

        /// <summary>
        /// 初期化はここで行う
        /// </summary>
        protected virtual void InitializeScene()
        {
            // シーンの初期化処理を記述
        }

        /// <summary>
        /// シーンのサービスを登録
        /// </summary>
        protected virtual void RegisterSceneServices()
        {
            // シーンごとのサービス登録処理を記述
        }
        
        /// <summary>
        /// シーンを読み込む
        /// </summary>
        /// <param name="sceneName"></param>
        protected virtual void LoadTargetScene(string sceneName)
        {
            // シーン遷移処理を記述
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// 共通シーンのロード
        /// </summary>
        public void LoadCommonScene()
        {
            // 共通シーンのロード
            SceneManager.LoadScene("CommonScene");
        }

        /// <summary>
        /// タイトルシーンのロード
        /// </summary>
        public void LoadTitleScene()
        {
            // タイトルシーンのロード
            SceneManager.LoadScene("Title");
        }
    }
}