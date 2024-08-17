using System;
using UnityEngine;
using Utils;

namespace Scene.Common
{
    public class CommonScene
        : MonoBehaviour
    {
        [SerializeField] private ObjectPool floorPool;
        [SerializeField] private ObjectPool wallPool;
        [SerializeField] private ObjectPool enemyPool;
        [SerializeField] private ObjectPool item01Pool;
        
        private void Awake()
        {
            // プールの初期化  
            InitializeObjectPools();
            
            // サービスの登録
            RegisterSceneServices();
        }

        private void RegisterSceneServices()
        {
            // 何かサービス登録あれば実行
        }

        private void InitializeObjectPools()
        {
            // 何か初期設定あれば実行
        }
    }
}