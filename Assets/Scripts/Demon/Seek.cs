namespace Demon
{
    public class Seek : IState
    {
        public void Enter(IContext context)
        {
            //
        
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {

        }

        public bool CanTransitTo(IState nextState)
        {
            return false;
        }

    }
}
