namespace Manager.Service
{
    public class PlayerHpService
    {
        public int CurrentHp { get; private set; }
        
        private bool _isInitialized;
        
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="initialHp"></param>
        public void Initialize(int initialHp)
        {
            if (_isInitialized)
            {
                return;
            }
            
            CurrentHp = initialHp;
            _isInitialized = true;
        }
        
        /// <summary>
        /// HPを設定
        /// </summary>
        /// <param name="hp"></param>
        public void SetHp(int hp)
        {
            CurrentHp = hp;
        }
    }
}