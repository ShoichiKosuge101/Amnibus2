using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// オブジェクトプール
    /// </summary>
    public class ObjectPool
        : MonoBehaviour
    {
        public GameObject prefab;
        private readonly Queue<GameObject> _pool = new Queue<GameObject>();
        
        /// <summary>
        /// プールの親オブジェクト
        /// </summary>
        [SerializeField]
        private Transform poolParent;
        
        /// <summary>
        /// プールからオブジェクトを取得
        /// </summary>
        /// <returns></returns>
        public GameObject GetObject(Transform target = null)
        {
            // targetが存在する場合は上書き
            var parent = target ?? poolParent;
            
            if(_pool.Count == 0)
            {
                return Instantiate(prefab, parent);
            }
            
            var obj = _pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        /// <summary>
        /// オブジェクトをプールに返却
        /// </summary>
        /// <param name="obj"></param>
        public void ReleaseObject(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(poolParent);
            _pool.Enqueue(obj);
        }

    }
}