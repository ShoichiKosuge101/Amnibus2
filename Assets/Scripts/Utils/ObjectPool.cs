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
        /// プールからオブジェクトを取得
        /// </summary>
        /// <returns></returns>
        public GameObject GetObject()
        {
            if(_pool.Count == 0)
            {
                return Instantiate(prefab);
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
            _pool.Enqueue(obj);
        }
    }
}