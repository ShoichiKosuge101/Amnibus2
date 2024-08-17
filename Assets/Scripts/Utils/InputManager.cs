using System;
using UniRx;
using UnityEngine;
using Utils.Interface;

namespace Utils
{
    /// <summary>
    /// 入力マネージャー
    /// </summary>
    public class InputManager
        : IInputManager
    {
        public IObservable<long> DecideObservable => Observable
            .EveryUpdate()
            .Where(_ => Input.GetButtonDown("Fire1"));
        
        /// <summary>
        /// 入力を監視するObservable
        /// </summary>
        public IObservable<Vector2> InputObservable => Observable
            .EveryUpdate()
            .Select(_ =>
            {
                // 入力を取得
                var vector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                
                // 斜め入力は不可なので、xが入力されていたらyは0にする
                if (vector.x != 0)
                {
                    vector.y = 0;
                }
                return vector.normalized;
            })
            .Where(v => v.sqrMagnitude > 0.1f);
    }
}