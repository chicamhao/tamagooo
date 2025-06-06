using UnityEngine;
using UnityEngine;

namespace Demon
{
    public sealed class WanderState : MonoBehaviour, IState
    {
        StateContext _context;

        public void Enter(StateContext context)
        {
            _context = context;
        }

        public void Update()
        {
            if (_context == null) return;

            var path = _context.PatrolPath;
            Random.Range(0, 3);
        }

        public void Exit()
        {
            _context = null;
        }
    }
}