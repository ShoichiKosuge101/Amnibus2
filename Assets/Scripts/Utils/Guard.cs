using UnityEngine;

namespace Utils
{
    public static class Guard
    {
        public static bool IsObjectDestroyed(UnityEngine.Object obj)
        {
            return obj == null || (obj is GameObject gameObject && !gameObject.activeInHierarchy);
        }
    }
}