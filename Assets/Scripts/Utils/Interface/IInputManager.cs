using System;
using UniRx;
using UnityEngine;

namespace Utils.Interface
{
    public interface IInputManager
    {
        IObservable<long> DecideObservable { get; }
        IObservable<Vector2> InputObservable { get; }
        
        IObservable<bool> OnSwitchInventoryOpenRx { get; }
        IObservable<bool> OnUseItemRx { get; }
    }
}