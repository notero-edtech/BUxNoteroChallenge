using System;

namespace ForieroEngine.Threading.Unity
{
    public interface IThreadRunner
    {
        void Execute(Action action);
    }
}