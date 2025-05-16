using UnityEngine;

namespace Demon
{
    public sealed class StateContext : MonoBehaviour, IContext
    {
        [SerializeField] GameObject _playerObject;
        public GameObject PlayerObject => _playerObject;

        private IState[] _states;
        private IState _currentState;

        public void Start()
        {
            _states = new IState[]
            {
                new Idle(),
                new Seek()
            };
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void ChangeState()
        {
            if (TrySearchNextState(out var newState))
            {
                _currentState?.Exit();
                _currentState = newState;
                _currentState.Enter(this);
            }
        }

        private bool TrySearchNextState(out IState nextState)
        {
            nextState = _currentState;
            foreach (var state in _states)
            {
                if (_currentState.CanTransitTo(state))
                {
                    nextState = state;
                    return true;
                }
            }
            return false;
        }
    }   
}