namespace Demon
{
    public class Idle : IState
    {
        public bool CanTransitTo(IState nextState)
        {
            if (nextState is Seek)
            {
                return true;
            }
            return false;
        }

        public void Enter(IContext context)
        {
            // play animation.
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}
