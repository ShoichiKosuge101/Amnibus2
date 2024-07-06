using UnityEngine;

namespace Utils
{
    /// <summary>
    /// シングルトンジェネリック
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> 
        : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }
        
        protected virtual bool IsPersistant => true;
        
        public static bool IsInitialized => Instance != null;
        
        /// <summary>
        /// インスタンスが存在しない場合は生成
        /// </summary>
        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = (T)this;
            if (IsPersistant)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        
        /// <summary>
        /// 破棄時にインスタンスをnullにする
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
