using Unity.Cinemachine;
using UnityEngine;

namespace Utils
{
    public class CameraInitializer
    {
        /// <summary>
        /// フォロー対象を設定
        /// </summary>
        /// <param name="virtualCemera"></param>
        /// <param name="target"></param>
        public void SetupFollow(CinemachineCamera virtualCemera, GameObject target)
        {
            virtualCemera.Follow = target.transform;
        }
    }
}