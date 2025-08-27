using UnityEngine;
using System;

namespace Doulingo.Input
{
    public interface IInputReader
    {
        event Action<int> OnKeyDown;  // keyIndex
        event Action<int, float> OnKeyUp;  // keyIndex, holdTime
        
        void Initialize();
        void Tick();
        void Cleanup();
    }
}
