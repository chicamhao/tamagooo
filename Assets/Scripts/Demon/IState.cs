namespace Control.Monsters
{
    public interface IState
    {
        void Enter(StateContext context);
        void Update();
        void Exit();
    }
}