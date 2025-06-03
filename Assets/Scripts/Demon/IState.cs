namespace Demon
{
    public interface IState
    {
        void Enter(StateContext context);
        void Update();
        void Exit();
    }
}