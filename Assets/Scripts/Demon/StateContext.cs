namespace Demon
{
    public sealed class StateContext
    {
        private IState _currentState;

        public PatrolPath PatrolPath => _patrolPath;
        PatrolPath _patrolPath;

        public void Start(IState state, PatrolPath patrolPath)
        {
            _currentState = state;
            _patrolPath = patrolPath;
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