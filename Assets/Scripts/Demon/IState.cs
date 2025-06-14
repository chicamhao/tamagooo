namespace Demon
{
    public interface IState
    {
        void Enter(StateControl control);
        void Update();
        void Exit();
    }
}