namespace Control.Monsters
{
    public sealed class StateContext
    {
        private IState _currentState;
        
        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter(this);
        }

        private void Update()
        {
            _currentState?.Update();
        }
    }
}