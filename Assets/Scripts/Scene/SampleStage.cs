using Dungeons;
using Scene.Common;
using Utils;
using Utils.Interface;

namespace Scene
{
    public class SampleStage
        : SceneBase
    {
        protected override void RegisterSceneServices()
        {
            // 入力マネージャーを登録
            ServiceLocator.Instance.Register<IInputManager>(new InputManager());
        }
        
        protected override void InitializeScene()
        {
            // シーンの初期化処理を記述
            
            // DgGeneratorを生成
            var dgGenerator = new DgGenerator();
        }
    }
}