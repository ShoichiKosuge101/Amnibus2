using UnityEngine;

namespace Controller.Animator
{
    public class PlayerAnimatorController
        : MonoBehaviour
    {
        private UnityEngine.Animator _animator;
        private static readonly int IsUp = UnityEngine.Animator.StringToHash("IsUp");
        private static readonly int IsDown = UnityEngine.Animator.StringToHash("IsDown");
        private static readonly int IsRight = UnityEngine.Animator.StringToHash("IsRight");
        private static readonly int IsLeft = UnityEngine.Animator.StringToHash("IsLeft");

        private void Start()
        {
            _animator = GetComponent<UnityEngine.Animator>();
        }
        
        /// <summary>
        /// 方向キーに応じて、SetBool
        /// </summary>
        /// <param name="direction"></param>
        public void SetKeyDirection(Vector2 direction)
        {
            // 一旦全てのフラグをfalseにする
            _animator.SetBool(IsUp,false);
            _animator.SetBool(IsDown,false);
            _animator.SetBool(IsRight,false);
            _animator.SetBool(IsLeft,false);
            
            if(direction.y < 0)
            {
                _animator.SetBool(IsDown,true);
                return;
            }
            
            if (direction.y > 0)
            {
                _animator.SetBool(IsUp,true);
                return;
            }
            
            if (direction.x > 0)
            {
                _animator.SetBool(IsRight,true);
                return;
            }
            
            if (direction.x < 0)
            {
                _animator.SetBool(IsLeft,true);
                return;
            }
            
            Debug.LogWarning("Trigger not found");
        }
    }
}