using System;
using UnityEngine;

namespace Utils.Interface
{
    public interface IInputManager
    {
        IObservable<long> DecideObservable { get; }
        IObservable<Vector2> InputObservable { get; }
    }
}