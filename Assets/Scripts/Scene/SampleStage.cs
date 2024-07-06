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
    }
}