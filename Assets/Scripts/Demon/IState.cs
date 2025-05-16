using UnityEngine;

namespace Demon
{
    public interface IState : ICondition
    {
        void Enter(IContext context);
        void Update();
        void Exit();
    }

    public interface ICondition
    {
        bool CanTransitTo(IState nextState);
    }

    public interface IContext
    {
        GameObject PlayerObject { get; }
    }
}