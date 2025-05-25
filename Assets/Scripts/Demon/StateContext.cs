namespace Control.Monsters
{
    public sealed class StateContext
    {
        private IState _currentState;

        public void Start(IState state)
        {
            _currentState = state;
        }

        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter(this);
        }

        public void Update()
        {
            _currentState?.Update();
        }
    }
} 