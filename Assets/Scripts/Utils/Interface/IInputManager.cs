using System;
using UnityEngine;

namespace Utils.Interface
{
    public interface IInputManager
    {
        IObservable<Vector2> InputObservable { get; }
    }
}