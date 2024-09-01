namespace Scene
{
    public class StageManager
    {
        public enum Stage
        {
            TITLE   = 0,
            STAGE_1 = 1,
            STAGE_2 = 2,
            STAGE_3 = 3,
        }
        
        private const string STAGE_1_SCENE_NAME = "Stage1";
        private const string STAGE_2_SCENE_NAME = "Stage2";
        private const string STAGE_3_SCENE_NAME = "Stage3";
        private const string TITLE_SCENE_NAME = "Title";
        
        // シーン名からステージを取得
        public static Stage GetStage(string sceneName)
        {
            return sceneName switch
            {
                STAGE_1_SCENE_NAME => Stage.STAGE_1,
                STAGE_2_SCENE_NAME => Stage.STAGE_2,
                STAGE_3_SCENE_NAME => Stage.STAGE_3,
                TITLE_SCENE_NAME   => Stage.TITLE,
                _                  => Stage.TITLE,
            };
        }
        
        public static string GetSceneName(Stage stage)
        {
            return stage switch
            {
                Stage.STAGE_1 => STAGE_1_SCENE_NAME,
                Stage.STAGE_2 => STAGE_2_SCENE_NAME,
                Stage.STAGE_3 => STAGE_3_SCENE_NAME,
                _             => TITLE_SCENE_NAME,
            };
        }
        
        // 次のシーン名を取得
        public static string GetNextSceneName(Stage stage)
        {
            return stage switch
            {
                Stage.TITLE   => STAGE_1_SCENE_NAME,
                Stage.STAGE_1 => STAGE_2_SCENE_NAME,
                Stage.STAGE_2 => STAGE_3_SCENE_NAME,
                Stage.STAGE_3 => TITLE_SCENE_NAME,
                _             => TITLE_SCENE_NAME,
            };
        }
    }
}